using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class ChatSessionMember
	{
		public int ChatSessionId { get; set; }
		public ChatSession ChatSession { get; set; }
		public int MemberId { get; set; }
		public Member Member { get; set; }
	}
}
