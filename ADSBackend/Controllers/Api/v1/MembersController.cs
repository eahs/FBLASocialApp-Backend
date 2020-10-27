using ADSBackend.Data;
using ADSBackend.Helpers;
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADSBackend.Controllers.Api.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/members")]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public MembersController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;            
        }

        private static bool IsValidEmail(string email)
        {
            // source: http://thedailywtf.com/Articles/Validating_Email_Addresses.aspx
            Regex rx = new Regex(@"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ApiResponse> Authenticate(AuthenticateRequest model)
        {
            var member = await _userService.Authenticate(model);

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
        /// <param name="member"></param>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> CreateMember ([Bind ("FirstName,LastName,Birthday,Email,Password")]Member member)
        {
            PasswordHash ph = PasswordHasher.Hash(member.Password);

            var safemember = new Member
            {
                Email = member.Email.Trim(),
                FirstName = member.FirstName.Trim(),
                LastName = member.LastName.Trim(),
                Birthday = member.Birthday,
                Password = ph.HashedPassword,
                PasswordSalt = ph.Salt,
                Country = "US"
            };

            // Validate email
            if (safemember.Email.Length == 0 || !IsValidEmail(safemember.Email))
                return new ApiResponse(System.Net.HttpStatusCode.Forbidden, member, "Email address is invalid");

            // Validate password (check member since safemember is already hashed)
            if (member.Password.Length < 8)
                return new ApiResponse(System.Net.HttpStatusCode.Forbidden, member, "Password must be at least 8 characters");

            // Check to see if member already exists
            var _membercheck = await _context.Member.FirstOrDefaultAsync(m => m.Email == safemember.Email);

            if (_membercheck != null)
                return new ApiResponse(System.Net.HttpStatusCode.Forbidden, member, "An account for this email already exists");

            // Passed checks so create member
            _context.Member.Add(safemember);
            await _context.SaveChangesAsync();

            return new ApiResponse(System.Net.HttpStatusCode.OK, safemember);
        }

        // PUT: api/v1/Members/{id}
        /// <summary>
        /// Update an existing member
        /// </summary>
        /// <param name="id"></param>   
        /// <param name="item"></param>   
        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateMember(int id, [Bind("MemberId, FirstName, LastName, Birthday, Gender, Address, City, State, ZipCode, Country, PhoneNumber, profileImageSource, Description")]Member member)
        {
            var httpUser = (Member) HttpContext.Items["User"];
            var newMember = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);
            if (newMember == null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.NotFound);
            } else if(newMember.MemberId != member.MemberId) // If the logged in user is not the same as the user they are trying to change
            {
                return new ApiResponse(System.Net.HttpStatusCode.Forbidden);
            }
            
            if(member.FirstName != null)
			{
                newMember.FirstName = member.FirstName;
			}
            if(member.LastName != null)
			{
                newMember.LastName = member.LastName;
			}
            if(member.Birthday != null)
            {
                DateTime date = Convert.ToDateTime(member.Birthday);
                newMember.Birthday = date;
			}
            if(member.Gender != null)
			{
                newMember.Gender = member.Gender;
			}
            if(member.Address != null)
			{
                newMember.Address = member.Address;
			}
            if(member.City != null)
			{
                newMember.City = member.City;
			}
            if(member.State != null)
			{
                newMember.State = member.State;
			}
            if(member.ZipCode != null)
			{
                newMember.ZipCode = member.ZipCode;
			}

            if (member.Country != null)
            {
                newMember.Country = member.Country;
            }

            if (member.PhoneNumber != null)
            {
                newMember.PhoneNumber = member.PhoneNumber;
            }

            if (member.profileImageSource != null)
            {
                newMember.profileImageSource = member.profileImageSource;
            }

            if (member.Description != null)
            {
                newMember.Description = member.Description;
            }

            newMember.FullName = newMember.FirstName + " " + newMember.LastName;

            return new ApiResponse(System.Net.HttpStatusCode.OK, newMember);
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