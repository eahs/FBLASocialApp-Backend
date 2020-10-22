using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class ChatMessage
	{
		public int MessageId { get; set; }
		public Member Author { get; set; }
		public Member To { get; set; }
		public string Body { get; set; }
		public string Image { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime EditedAt { get; set; }
		public bool isDeleted { get; set; } = false;
	}
}
