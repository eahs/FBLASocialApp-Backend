using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class PostPhoto
    {
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }

        public int Order { get; set; } = 0;
    }
}
