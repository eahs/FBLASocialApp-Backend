using System;
using System.Collections.Generic;
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
		List<ChatMessage> Messages { get; set; }
		public DateTime LastMessageSentAt { get; set; }
	}
}
