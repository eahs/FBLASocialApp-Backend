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

        // GET: api/v1/posts/{id}
        /// <summary>
        /// Returns the list of post from a specific member
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetPosts(int id)
        {

            var posts = await _context.Post.Where(p => p.AuthorId == id)
                .Include(p => p.Author)
                .ThenInclude(m => m.Friends)
                .Include(p => p.Reactions)
                .ThenInclude(r => r.Reaction)
                .ToListAsync();
            System.Diagnostics.Debug.WriteLine(posts);
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
            return new ApiResponse(System.Net.HttpStatusCode.OK, safePost);
        }

        // PUT: api/v1/posts/
        /// <summary>
        /// Updates an existing post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ApiResponse> UpdatePost([Bind(UpdatePostBindingFields)] Post post)
        {
            return new ApiResponse(System.Net.HttpStatusCode.OK, null);
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
    }
}