using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class Comment
	{
		[Key]
		public int CommentId { get; set; }
		public int AuthorId { get; set; }
		public Member Author { get; set; }
		public int ParentCommentId { get; set; }
		public string Body { get; set; }
		public string AdditionalData { get; set; } = "{}";
		public DateTime CreatedAt { get; set; }
		public DateTime EditedAt { get; set; }

		public List<Reaction> Reactions { get; set; }

	}
}
