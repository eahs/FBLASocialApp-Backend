using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class ChatSession
	{
		public int ChatSessionId { get; set; }
		public List<Member> ChatMembers { get; set; }
		public List<ChatMessage> Messages { get; set; }
		[NotMapped]
		public ChatMessage LastMessage { get; set; }
	}
}
