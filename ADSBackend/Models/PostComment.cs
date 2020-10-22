using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class PostComment
	{
		public int PostId { get; set; }
		public Post Post { get; set; }
		public int CommentId { get; set; }
		public Comment Comment { get; set; }
	}
}
