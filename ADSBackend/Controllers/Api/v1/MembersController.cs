using ADSBackend.Data;
using ADSBackend.Helpers;
using ADSBackend.Models;
using ADSBackend.Models.ApiModels;
using ADSBackend.Models.AuthenticationModels;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YakkaApp.Helpers;

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
            var httpUser = (Member)HttpContext.Items["User"];

            var member = await _context.Member.Include(m => m.Friends)
                                              .ThenInclude(f => f.Friend)
                                              .FirstOrDefaultAsync(m => m.MemberId == id);

            // TODO: Strip all data that isn't supposed to be public from this api response

            return new ApiResponse(System.Net.HttpStatusCode.OK, member);  
        }

        private const string createMemberBindingFields = "FirstName,LastName,Birthday,Email,Password,Country";
        private const string updateMemberBindingFields = "FirstName, LastName, Birthday, Gender, Address, City, State, ZipCode, Country, PhoneNumber, profileImageSource, Description";

        // POST: api/v1/Members/
        /// <summary>
        /// Create a new member
        /// </summary>
        /// <param name="member"></param>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> CreateMember ([Bind (createMemberBindingFields)]Member member)
        {
            var safemember = new Member
            {
                Email = member.Email?.Trim() ?? "",
                FirstName = member.FirstName?.Trim() ?? "",
                LastName = member.LastName?.Trim() ?? "",
                Birthday = member.Birthday,
                Password = member.Password ?? "",                
                Country = member.Country ?? "US"
            };
            
            TryValidateModel(safemember);
            ModelState.Scrub(createMemberBindingFields);  // Remove all errors that aren't related to the binding fields

            if (!ModelState.IsValid)
            {
                // Return all validation errors
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An error has occurred", ModelState);
            }

            // Check to see if member already exists
            var _membercheck = await _context.Member.FirstOrDefaultAsync(m => m.Email == safemember.Email);
            if (_membercheck != null)
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An account for this email already exists");

            // Securely hash the member password
            PasswordHash ph = PasswordHasher.Hash(member.Password ?? "");
            safemember.Password = ph.HashedPassword;
            safemember.PasswordSalt = ph.Salt;

            // Create a new wall for this member
            var wall = new Wall();

            // Passed checks so create member
            _context.Wall.Add(wall);
            await _context.SaveChangesAsync();

            safemember.WallId = wall.WallId;

            _context.Member.Add(safemember);
            await _context.SaveChangesAsync();

            return new ApiResponse(System.Net.HttpStatusCode.OK, safemember);
        }

        // PUT: api/v1/members/
        /// <summary>
        /// Update an existing member
        /// </summary>
        /// <param name="member"></param>   
        [HttpPut]
        public async Task<ApiResponse> UpdateMember([Bind(updateMemberBindingFields)]Member member)
        {
            var httpUser = (Member) HttpContext.Items["User"];
            var newMember = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);
            
            if (newMember == null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, null, "User not found");
            }
            
            newMember.FirstName = member.FirstName ?? newMember.FirstName;
            newMember.LastName = member.LastName ?? newMember.LastName;
            newMember.Birthday = member.Birthday; // TODO: Check to see if the birthday is valid
            newMember.Address = member.Address ?? newMember.Address;
            newMember.Gender = member.Gender ?? newMember.Gender;
            newMember.City = member.City ?? newMember.City;
            newMember.State = member.State ?? newMember.State;
            newMember.ZipCode = member.ZipCode ?? newMember.ZipCode;
            newMember.Country = member.Country ?? newMember.Country;
            newMember.PhoneNumber = member.PhoneNumber ?? newMember.PhoneNumber;
            newMember.profileImageSource = member.profileImageSource ?? newMember.profileImageSource;
            newMember.Description = member.Description ?? newMember.Description;

            TryValidateModel(newMember);
            ModelState.Scrub(updateMemberBindingFields);  // Remove all errors that aren't related to the binding fields

            // Add custom errors to fields
            //ModelState.AddModelError("Email", "Something else with email is wrong");
          
            if (!ModelState.IsValid)
            {
                // Return all validation errors
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An error has occurred", ModelState);
            }

            _context.Member.Update(newMember);
            await _context.SaveChangesAsync();
            
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