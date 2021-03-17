﻿using ADSBackend.Data;
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
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FileTypeChecker;
using FileTypeChecker.Abstracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

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

        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ApiResponse> Authenticate(AuthenticateRequest model)
        {
            var member = await _userService.Authenticate(model);

            if (member == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, model, "Email or password is incorrect");

            return new ApiResponse(System.Net.HttpStatusCode.OK, member);
        }

        /// <summary>
        /// Gets a list of friends for a member
        /// </summary>
        /// <returns></returns>
        [HttpGet("friends")]
        public async Task<ApiResponse> GetFriends()
        {
            var httpUser = (Member) HttpContext.Items["User"];
            //
            // var member = await _context.Member.Include(m => m.Friends)
            //     .ThenInclude(mf => mf.Friend)
            //     .ThenInclude(mf => mf.ProfilePhoto)
            //     .FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);
            //
            // if (member == null)
            //     return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Member not found");
            //
            // List<Member> friends = member.Friends.Select(f => f.Friend).ToList();
            //
            // for (int i = 0; i < friends.Count; i++)
            // {
            //     Member rf = friends[i];
            //
            //     friends[i] = new Member
            //     {
            //         MemberId = rf.MemberId,
            //         FirstName = rf.FirstName,
            //         LastName = rf.LastName,
            //         ProfilePhoto = rf.ProfilePhoto
            //     };
            // }
            //
            // return new ApiResponse(System.Net.HttpStatusCode.OK, friends);
            
            // TEMP WORK AROUND FOR DEMO #2.......

            var members = await _context.Member.Include(m => m.Friends)
                .Include(m => m.Wall)
                .Include(m => m.ProfilePhoto)
                .Include(m => m.ChatSessions)
                .Include(m => m.FriendRequests).ToListAsync();
            if(members == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Member(s) not found");
            
            return new ApiResponse(System.Net.HttpStatusCode.OK, members);
        }

        [HttpPost("friends/{id}")]
        public async Task<ApiResponse> AddFriendRequest(int id)
        {
            var httpUser = (Member) HttpContext.Items["User"];

            var member = await _context.Member.Include(m => m.Friends)
                .FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);

            if (member == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Member not found");

            var exists = member.Friends.Where(f => f.FriendId == id);

            if (exists != null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, errorMessage: "Member already a friend");
            }

            FriendRequest request = new FriendRequest
            {
                MemberId = httpUser.MemberId,
                FriendId = id,
                RequestIssuedAt = DateTime.Now,
                Status = FriendRequestStatus.Pending
            };

            _context.FriendRequest.Add(request);
            await _context.SaveChangesAsync();

            return new ApiResponse(System.Net.HttpStatusCode.OK, request);
        }

        [HttpDelete("friends/{id}")]
        public async Task<ApiResponse> RemoveFriend(int id)
        {
            var httpUser = (Member) HttpContext.Items["User"];

            var member = await _context.Member.Include(m => m.Friends)
                .FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);

            if (member == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Member not found");

            var friend = member.Friends.FirstOrDefault(f => f.FriendId == id);

            if (friend != null)
            {
                // Remove friend
                _context.MemberFriend.Remove(friend);
                await _context.SaveChangesAsync();
            }

            // Remove any pending friend requests
            var requests = await _context.FriendRequest.Where((fr =>
                    fr.MemberId == httpUser.MemberId && fr.FriendId == id && fr.Status == FriendRequestStatus.Pending))
                .ToListAsync();

            foreach (var request in requests)
            {
                request.Status = FriendRequestStatus.Rejected;
            }

            _context.FriendRequest.UpdateRange(requests);
            await _context.SaveChangesAsync();

            return new ApiResponse(System.Net.HttpStatusCode.OK);
        }

        [HttpGet("friends/requests")]
        public async Task<ApiResponse> GetFriendRequests()
        {
            var httpUser = (Member) HttpContext.Items["User"];

            var member = await _context.Member.Include(m => m.FriendRequests)
                .ThenInclude(mf => mf.Friend)
                .Include(m => m.ProfilePhoto)
                .FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);

            if (member == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Member not found");

            foreach (var request in member.FriendRequests)
            {
                request.Friend = new Member
                {
                    MemberId = request.Friend.MemberId,
                    FirstName = request.Friend.FirstName,
                    LastName = request.Friend.LastName,
                    ProfilePhoto = request.Friend.ProfilePhoto
                };
            }

            return new ApiResponse(System.Net.HttpStatusCode.OK, member.FriendRequests);
        }

        // GET: api/v1/Members/{id}
        /// <summary>
        /// Returns a specific member
        /// </summary>
        /// <param name="id"></param>    
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetMember(int id)
        {
            // TODO: Add validation for an id that member is a friend of the logged in user?
            var httpUser = (Member) HttpContext.Items["User"];

            var member = await _context.Member.Include(m => m.Friends)
                .ThenInclude(f => f.Friend)
                .ThenInclude(mf => mf.ProfilePhoto)
                .Include(m => m.ProfilePhoto)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            for (int i = 0; i < member.Friends.Count; i++)
            {
                Member rf = member.Friends[i].Friend;

                member.Friends[i] = new MemberFriend
                {
                    Friend = new Member
                    {
                        MemberId = rf.MemberId,
                        FirstName = rf.FirstName,
                        LastName = rf.LastName,
                        ProfilePhoto = rf.ProfilePhoto
                    }
                };
            }

            if (member == null)
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, errorMessage: "Member not found");

            return new ApiResponse(System.Net.HttpStatusCode.OK, member);
        }

        private const string CreateMemberBindingFields = "FirstName,LastName,Birthday,Email,Password,Country";

        private const string UpdateMemberBindingFields =
            "FirstName,LastName,Birthday,Gender,Address,City,State,ZipCode,Country,PhoneNumber,Description";

        // POST: api/v1/Members/
        /// <summary>
        /// Create a new member
        /// </summary>
        /// <param name="member"></param>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> CreateMember([Bind(CreateMemberBindingFields)] Member member)
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
            ModelState.Scrub(CreateMemberBindingFields); // Remove all errors that aren't related to the binding fields

            if (!ModelState.IsValid)
            {
                // Return all validation errors
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An error has occurred", ModelState);
            }

            // Check to see if member already exists
            var _membercheck = await _context.Member.FirstOrDefaultAsync(m => m.Email == safemember.Email);
            if (_membercheck != null)
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null,
                    "An account for this email already exists");

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
        public async Task<ApiResponse> UpdateMember([Bind(UpdateMemberBindingFields)] Member member)
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
            newMember.Description = member.Description ?? newMember.Description;

            TryValidateModel(newMember);
            ModelState.Scrub(UpdateMemberBindingFields); // Remove all errors that aren't related to the binding fields

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
        public async Task<ApiResponse> DeleteMember(int id)
        {
            var httpUser = (Member) HttpContext.Items["Users"];
            var member = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);
            if (member == null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, null, "Member not found");
            }

            _context.Member.Remove(member);
            await _context.SaveChangesAsync();
            return new ApiResponse(System.Net.HttpStatusCode.OK, null);
        }

        // PUT: api/v1/Members/image
        /// <summary>
        /// Updates a Member's Profile Picture
        /// </summary>
        /// <param name="file"></param>   
        [HttpPut("image")]
        public async Task<object> UpdateProfileImage(IFormFile file)
        {
            var httpUser = (Member) HttpContext.Items["Users"];
            var member = await _context.Member.FirstOrDefaultAsync(a => a.MemberId == httpUser.MemberId);


            if (member == null)
            {
                return new
                {
                    Status = "Failed"
                };
            }
            
            var fileType = System.IO.Path.GetExtension(file.FileName);
            string[] allowedExtensions = new string[] {".png", ".jpg"};
            if (allowedExtensions.Contains(fileType)) // If the file type is a png or jpg
            {
                string TEMP_PATH = Path.Combine("wwwroot/images/members/" + httpUser.MemberId + "temp." + fileType);
                string FINAL_PATH = Path.Combine("wwwroot/images/members/" + httpUser.MemberId + ".jpg");
                string BASE_PFP_URL = "https://yakka.tech/images/members/";

                await file.CopyToAsync(System.IO.File.Create(TEMP_PATH)); // Creates the Temp File
                Image image = Image.Load(TEMP_PATH);
                int cropWidth = Math.Min(image.Width, image.Height);
                int x = image.Width / 2 - cropWidth / 2;
                int y = image.Height / 2 - cropWidth / 2;

                // Crops the photo into a square
                image.Mutate(a => a
                    .Crop(new Rectangle(x, y, cropWidth, cropWidth))
                    .Resize(150, 150));


                // Save the new one
                image.SaveAsJpeg(FINAL_PATH); // Automatic encoder selected based on extension.    

                // Delete the temp file
                System.IO.File.Delete(TEMP_PATH); // Deletes the Temp File


              

                var pfp = new Photo();
                pfp.Filename = httpUser.MemberId + ".jpg";
                pfp.MemberId = httpUser.MemberId;
                pfp.Member = member;
                pfp.Url = BASE_PFP_URL + httpUser.MemberId + ".jpg";
                pfp.CreatedAt = new DateTime();

                member.ProfilePhoto = pfp; // Assign the new PFP to the member.

                _context.Member.Update(member);
                await _context.SaveChangesAsync();
            }

            return new
            {
                Status = "Success"
            };
        }

        // Post Upload Async Method
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return Ok(new {count = files.Count, size});
        }
    }
}