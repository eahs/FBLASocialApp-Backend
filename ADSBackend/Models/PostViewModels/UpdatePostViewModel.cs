using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{

    public class UpdatePostViewModel
    {
        [Required]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public PrivacyLevel PrivacyLevel { get; set; }
        public bool IsFeatured { get; set; } = false;

    }
}