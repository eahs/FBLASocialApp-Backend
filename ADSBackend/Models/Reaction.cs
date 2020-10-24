using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class Reaction
	{
		[Key]
		public int ReactionId { get; set; }
		public int ReactionType { get; set; }
		public int MemberId { get; set; }
		public Member Member { get; set; }
	}
}
