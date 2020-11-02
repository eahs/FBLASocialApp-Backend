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
        [Key]
        public int MemberId { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "First name is required")]  // Max 32 characters, min 1 character
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "Last name is required")]  // Max 32 characters, min 1 character
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")] 
        public DateTime Birthday { get; set; }

        public string FullName { 
            get { return FirstName + " " + LastName; } 
            set { }
        }

        public string Gender { get; set; } = "";

        public string Address { get; set; } = "";

        public string City { get; set; } = "";

        public string State { get; set; } = "PA";

        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Required]
        public string Country { get; set; } = "US";

        [Required, DataType(DataType.EmailAddress, ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        
        public int? ProfilePhotoId { get; set; }
        
        [Display(Name = "Profile Picture")]
        public Photo ProfilePhoto { get; set; }

        [Display(Name = "Bio")]
        public string Description { get; set; }

        [Required]
        [JsonIgnore]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }
        [JsonIgnore]

        public string PasswordSalt { get; set; }

        public int WallId { get; set; }
        public Wall Wall { get; set; }
        public List<MemberFriend> Friends { get; set; }
        public List<ChatSessionMember> ChatSessions { get; set; }
    }
}
