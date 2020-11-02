﻿using System.Threading.Tasks;
using ADSBackend.Controllers.Api.v1;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YakkaApp.Helpers;

namespace YakkaApp.Controllers.Api.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/posts")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        private const string CreatePostBindingFields = "AuthorId,Title,Body,Image,IsMachinePost,CreatedAt,PrivacyLevel,IsFeatured";
        private const string UpdatePostBindingFields = "AuthorId,Title,Body,EditedAt,PrivacyLevel,IsFeatured";

        public PostsController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/v1/posts/{memberId}
        /// <summary>
        /// Returns the list of post from a specific member
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpGet("{memberId}")]
        public async Task<ApiResponse> GetPosts(int memberId)
        {
            return new ApiResponse(System.Net.HttpStatusCode.OK, null);
        }
        
        // GET: api/v1/posts/{postId}
        /// <summary>
        /// Returns the list of post from a specific member
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet("{postId}")]
        public async Task<ApiResponse> GetPost(int postId)
        {
            return new ApiResponse(System.Net.HttpStatusCode.OK, null);
        }

        // POST: api/v1/posts/
        /// <summary>
        /// Creates a new post
        /// </summary>
        /// <param name="post"></param>
        [HttpPost]
        public async Task<ApiResponse> CreatePost([Bind(CreatePostBindingFields)]Post post)
        {
            var author = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == post.AuthorId);
            if (author == null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, post, "Please provide a valid AuthorId", ModelState);
            }
            var safePost = new Post
            {
                AuthorId = post.AuthorId,
                Title = post.Title ?? "",
                Body = post.Body ?? "",
                Image = post.Image ?? "",
                IsMachinePost = post.IsMachinePost, // Defaults to false in the model
                CreatedAt = post.CreatedAt, // Is required in the model, handled by the app.
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
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpDelete("{postId}")]
        public async Task<ApiResponse> DeletePost(int postId)
        {
            return new ApiResponse(System.Net.HttpStatusCode.OK, null);
        }
    }
}