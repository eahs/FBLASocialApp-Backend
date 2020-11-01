using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{

    public class PostViewModel
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Body { get; set; }
        public string Image { get; set; }
        public bool IsMachinePost { get; set; } = false;
        [Required]
        public DateTime CreatedAt { get; set; }
        public PrivacyLevel PrivacyLevel { get; set; } = PrivacyLevel.Public;
        public bool IsFeatured { get; set; } = false;

    }
}