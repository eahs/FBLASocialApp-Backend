using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class ChatMessage
	{
		[Key]
		public int MessageId { get; set; }
		public int AuthorId { get; set; }
		public Member Author { get; set; }
		public string Body { get; set; }
		public string Image { get; set; }
		public int ChatSessionId { get; set; }
		public ChatSession ChatSession { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime EditedAt { get; set; }
		public bool IsDeleted { get; set; } = false;
	}
}
