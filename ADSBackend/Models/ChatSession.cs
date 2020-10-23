using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class ChatSession
	{
		public int SessionId { get; set; }
		public Session Session { get; set; }
		public Member Member1 { get; set; }
		public Member Member2 { get; set; }
		public List<ChatMessage> Messages { get; set; }
		[NotMapped]
		public ChatMessage LastMessage { get; set; }
	}
}
