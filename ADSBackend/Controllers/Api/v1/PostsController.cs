using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADSBackend.Controllers.Api.v1;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using ADSBackend.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace ADSBackend.Controllers.Api.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/posts")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        private const string CreatePostBindingFields = "Title,Body,PrivacyLevel,IsFeatured";
        private const string UpdatePostBindingFields = "PostId,Title,Body,EditedAt,PrivacyLevel,IsFeatured";

        public PostsController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/v1/posts/home
        /// <summary>
        /// Gets the home timeline for a member
        /// </summary>
        /// <param name="page"></param>
        /// <param name="numPosts"></param>
        /// <returns></returns>
        [HttpGet("/home")]
        public async Task<ApiResponse> GetMemberHome(int page = 1, int numPosts = 25)
        {
            var httpUser = (Member)HttpContext.Items["User"];

            var member = await _context.Member.Include(m => m.Friends).FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);
            List<int> friendIds = member.Friends.Select(f => f.FriendId).ToList();

            var posts = await _context.Post.Where(p => friendIds.Contains(p.AuthorId) || p.IsMachinePost)
                .Include(p => p.Author)
                .Include(p => p.Reactions).ThenInclude(r => r.Reaction).ThenInclude(m => m.Member)
                .OrderByDescending(wp => wp.PostId)
                .Skip(page * numPosts)
                .Take(numPosts)
                .ToListAsync();

            ReducePostsResultset(posts);

            // System.Diagnostics.Debug.WriteLine(posts);

            return new ApiResponse(System.Net.HttpStatusCode.OK, posts);
        }

        // POST: api/v1/posts/
        /// <summary>
        /// Creates a new post
        /// </summary>
        /// <param name="post"></param>
        [HttpPost]
        public async Task<ApiResponse> CreatePost([Bind(CreatePostBindingFields)] Post post)
        {
            var httpUser = (Member) HttpContext.Items["User"];

            var member = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);
            if (member == null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, post, "Please provide a valid AuthorId",
                    ModelState);
            }

            var safePost = new Post
            {
                AuthorId = httpUser.MemberId,
                Title = post.Title ?? "",
                Body = post.Body ?? "",
                CreatedAt = new DateTime(), // Is required in the model, handled by the server
                PrivacyLevel = post.PrivacyLevel, // Defaults to 0(public) in the model
                IsFeatured = post.IsFeatured // Defaults to false in in the model
            };

            TryValidateModel(safePost);
            ModelState.Scrub(CreatePostBindingFields);

            if (!ModelState.IsValid)
            {
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An error has occured", ModelState);
            }

            _context.Post.Add(safePost);
            await _context.SaveChangesAsync();
            
            WallPost wp = new WallPost
            {
                PostId = safePost.PostId,
                WallId = httpUser.WallId
            };
            _context.WallPost.Add(wp);
            await _context.SaveChangesAsync();
            return new ApiResponse(System.Net.HttpStatusCode.OK, safePost);
        }

        // PUT: api/v1/posts/
        /// <summary>
        /// Updates an existing post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ApiResponse> UpdatePost([Bind(UpdatePostBindingFields)] UpdatePostViewModel post)
        {
            var httpUser = (Member) HttpContext.Items["User"];
            var newPost = await _context.Post.FirstOrDefaultAsync(p => p.PostId == post.PostId && p.AuthorId == httpUser.MemberId);
            if (newPost == null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, null, "Post not found");
            }

            newPost.Title = post.Title ?? newPost.Title;
            newPost.Body = post.Body ?? newPost.Body;
            newPost.EditedAt = new DateTime();
            newPost.PrivacyLevel = post.PrivacyLevel;
            newPost.IsFeatured = post.IsFeatured;

            TryValidateModel(newPost);
            ModelState.Scrub(UpdatePostBindingFields);

            if (!ModelState.IsValid)
            {
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An error has occurred", ModelState);
            }

            _context.Post.Update(newPost);
            await _context.SaveChangesAsync();
            return new ApiResponse(System.Net.HttpStatusCode.OK, newPost);
        }

        // DELETE: api/v1/posts/{postId}
        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeletePost(int id)
        {
            return new ApiResponse(System.Net.HttpStatusCode.OK, null);
        }

        /// <summary>
        /// Gets public posts for a given wall
        /// </summary>
        /// <param name="id">id of wall</param>
        /// <param name="page"></param>
        /// <param name="numPosts"></param>
        /// <returns></returns>
        [HttpGet("/walls/{id}")]
        public async Task<ApiResponse> GetWall(int id, int page = 1, int numPosts = 25)
        {
            var httpUser = (Member)HttpContext.Items["User"];

            // Look up the viewing member friends list
            var member = await _context.Member.Include(m => m.Friends).FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);
            List<int> friendIds = member.Friends.Select(f => f.FriendId).ToList();

            // Get all the wallposts for this wall
            var wallposts = await _context.WallPost.Where(w => w.WallId == id)
                            .Include(wp => wp.Post).ThenInclude(p => p.Author)
                            .Include(wp => wp.Post).ThenInclude(p => p.Reactions).ThenInclude(r => r.Reaction).ThenInclude(m => m.Member).ThenInclude(ph => ph.ProfilePhoto)
                            .OrderByDescending(wp => wp.PostId)
                            .ToListAsync();

            if (wallposts == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Wall not found");

            // Filter posts down to posts that are viewable
            List<Post> posts = wallposts.Where(wp => wp.Post.PrivacyLevel == PrivacyLevel.Public || (wp.Post.PrivacyLevel == PrivacyLevel.FriendsOnly && friendIds.Contains(wp.Post.AuthorId)))
                .Select(wp => wp.Post)
                .ToList();

            // Apply paging
            posts = posts.Skip(page * numPosts).Take(numPosts).ToList();

            // Reduce amount of data to transmit
            ReducePostsResultset(posts);

            //System.Diagnostics.Debug.WriteLine(posts);
            return new ApiResponse(System.Net.HttpStatusCode.OK, posts);
        }

        /// <summary>
        /// Gets the posts for a given member by id, respecting their privacy levels for each post
        /// </summary>
        /// <param name="id">id of member</param>
        /// <param name="page"></param>
        /// <param name="numPosts"></param>
        /// <returns></returns>
        [HttpGet("/walls/member/{id}")]
        public async Task<ApiResponse> GetMemberWall(int id, int page = 1, int numPosts = 25)
        {
            var httpUser = (Member)HttpContext.Items["User"];

            var member = await _context.Member.Include(m => m.Friends).FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Member not found");

            // Let's get the viewing member's list of friends by id
            List<int> friendIds = member.Friends.Select(f => f.FriendId).ToList();

            // Check to see if the viewing member is a friend of the wall owner - this is necessary in the event the
            // wall owner marks some posts as friends only
            bool isFriend = friendIds.Contains(httpUser.MemberId) || id == httpUser.MemberId;

            // Get all the wallposts for this member's wall
            var wallposts = await _context.WallPost.Where(w => w.WallId == member.WallId)
                            .Include(wp => wp.Post).ThenInclude(p => p.Author)
                            .Include(wp => wp.Post).ThenInclude(p => p.Reactions).ThenInclude(r => r.Reaction).ThenInclude(m => m.Member).ThenInclude(ph => ph.ProfilePhoto)
                            .OrderByDescending(wp => wp.PostId)
                            .ToListAsync();

            if (wallposts == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Wall not found");

            // Filter posts down to posts that are viewable
            List<Post> posts = wallposts.Where(wp => wp.Post.PrivacyLevel == PrivacyLevel.Public || 
                                                    (wp.Post.PrivacyLevel == PrivacyLevel.FriendsOnly && isFriend) ||
                                                    (wp.Post.PrivacyLevel == PrivacyLevel.Private && wp.Post.AuthorId == httpUser.MemberId))
                                   .Select(wp => wp.Post)
                                   .ToList();

            // Apply paging
            posts = posts.Skip(page * numPosts).Take(numPosts).ToList();

            ReducePostsResultset(posts);

            return new ApiResponse(System.Net.HttpStatusCode.OK, posts);
        }

        private void ReducePostsResultset (List<Post> posts)
        {
            foreach (var post in posts)
            {
                // Reduce member details to minimal amount
                post.Author = new Member
                {
                    MemberId = post.Author.MemberId,
                    FirstName = post.Author.FirstName,
                    LastName = post.Author.LastName,
                    ProfilePhoto = post.Author.ProfilePhoto
                };

                foreach (var reaction in post.Reactions)
                {
                    // Reduce member details to minimal amount
                    reaction.Reaction.Member = new Member
                    {
                        MemberId = reaction.Reaction.Member.MemberId,
                        FirstName = reaction.Reaction.Member.FirstName,
                        LastName = reaction.Reaction.Member.LastName,
                        ProfilePhoto = reaction.Reaction.Member.ProfilePhoto
                    };
                }
            }
        }

    }
}