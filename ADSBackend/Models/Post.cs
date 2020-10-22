using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class Post
	{
		[Key]
		public int PostId { get; set; }
		public int AuthorId { get; set; }
		public Member Author { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public string Image { get; set; }
		public bool isMachinePost { get; set; } = false;
		public DateTime CreatedAt { get; set; }
		public DateTime EditedAt { get; set; }
		public bool isDeleted { get; set; } = false;
		public List<PostReaction> Reactions { get; set; }
		public List<PostComment> Comments { get; set; }

	}
}
