using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public enum ReactionTypes
    {
		Like = 1,
		Love = 2,
		Surprised = 3,
		Curious = 4
    }

	public class Reaction
	{
		[Key]
		public int ReactionId { get; set; }
		public ReactionTypes ReactionType { get; set; } = ReactionTypes.Like;
		public int MemberId { get; set; }
		public Member Member { get; set; }
	}
}
