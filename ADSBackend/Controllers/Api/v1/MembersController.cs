using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.ApiModels;
using ADSBackend.Models.AuthenticationModels;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers.Api.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/members")]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IUserService _userService;

        public MembersController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ApiResponse> Authenticate(AuthenticateRequest model)
        {
            var member = _userService.Authenticate(model);

            if (member == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, model, "Username or password is incorrect");

            return new ApiResponse(System.Net.HttpStatusCode.OK, member);
        }

        // GET: api/v1/Members/{id}
        /// <summary>
        /// Returns a specific member
        /// </summary>
        /// <param name="id"></param>    
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetMember(int id)
        {
            //var member = await _context.Member.Include(m => m.Friends).ThenInclude(f => f.Friend).FirstOrDefaultAsync(m => m.MemberId == id);
            var member = new Member();

            return new ApiResponse(System.Net.HttpStatusCode.OK, member);  
        }

        // POST: api/v1/Members/
        /// <summary>
        /// Create a new member
        /// </summary>
        /// <param name="item"></param>   
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> CreateMember (Member member)
        {
            var safemember = member;

            // Validate email, password, firstname, lastname

            return new ApiResponse(System.Net.HttpStatusCode.OK, safemember);
        }

        // PUT: api/v1/Members/{id}
        /// <summary>
        /// Update an existing member
        /// </summary>
        /// <param name="id"></param>   
        /// <param name="item"></param>   
        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateMember(int id, Member member)
        {
            return new ApiResponse(System.Net.HttpStatusCode.OK, member);
        }

        // DELETE: api/v1/Members/{id}
        /// <summary>
        /// Delete a member
        /// </summary>
        /// <param name="id"></param>   
        [HttpDelete("{id}")]
        public async Task<bool> DeleteMember(int id)
        {
            return true;
        }
        

    }
}