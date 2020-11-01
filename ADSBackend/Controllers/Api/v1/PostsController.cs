using System.Threading.Tasks;
using ADSBackend.Controllers.Api.v1;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YakkaApp.Controllers.Api.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/posts")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        private const string CreatePostBindingFields = "PostId,AuthorId,Title,Body,Image,IsMachinePost,CreatedAt,PrivacyLevel,IsFeatured";
        private const string UpdatePostBindingFields = "PostId,Title,Body,EditedAt,PrivacyLevel,IsFeatured";

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
            return new ApiResponse(System.Net.HttpStatusCode.OK, null);
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
        [HttpDelete]
        public async Task<ApiResponse> DeletePost(int postId)
        {
            return new ApiResponse(System.Net.HttpStatusCode.OK, null);
        }
    }
}