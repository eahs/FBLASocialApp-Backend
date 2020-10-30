using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required]
        public DateTime Birthday { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required]
        public string Gender { get; set; }

        public string Address { get; set; }

        public string City { get; set; }
        
        public string State { get; set; } = "PA";

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        
        [Required]
        public string Country { get; set; } = "US";

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Display(Name = "Profile Picture")]
        public string profileImageSource { get; set; }

        [Display(Name = "Bio")]
        public string Description { get; set; }

        [Required]
        [JsonIgnore]
        public string Password { get; set; }
        
        [JsonIgnore]
        public string PasswordSalt { get; set; }

        public int WallId { get; set; }
        public Wall Wall { get; set; }
        public List<MemberFriend> Friends { get; set; }
        public List<ChatSessionMember> ChatSessions { get; set; }
    }
}
