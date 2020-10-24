using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class MemberFriend
	{
		public int MemberId { get; set; }
		public Member Member { get; set; }
		public int FriendId { get; set; }
		public Member Friend { get; set; }

	}
}
