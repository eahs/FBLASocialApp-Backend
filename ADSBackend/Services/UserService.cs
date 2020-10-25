using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ADSBackend.Helpers;
using ADSBackend.Models;
using ADSBackend.Models.AuthenticationModels;

namespace ADSBackend.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<Member> GetAll();
        Member GetById(int id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<Member> _members = new List<Member>
        {
            new Member { MemberId = 1, FirstName = "Test", LastName = "User", Email = "test@gmail.com", Password = "test" }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var member = _members.SingleOrDefault(x => x.Email == model.Email && x.Password == model.Password);
            //var member = _members.SingleOrDefault(x => x.Email == model.Email && x.Password == PasswordHasher.Hash(model.Password, x.PasswordSalt).HashedPassword);


            // return null if user not found
            if (member == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(member);

            return new AuthenticateResponse(member, token);
        }

        public IEnumerable<Member> GetAll()
        {
            return _members;
        }

        public Member GetById(int id)
        {
            return _members.FirstOrDefault(x => x.MemberId == id);
        }

        // helper methods

        private string generateJwtToken(Member user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.MemberId.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}