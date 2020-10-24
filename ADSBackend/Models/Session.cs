using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class Session
	{
		public int SessionId { get; set; }
		public int MemberId { get; set; }
		public Member Member { get; set; }
		public string Email { get; set; }
		public string Key { get; set; }
		public DateTime LastAccessedAt { get; set; }
	}
}
