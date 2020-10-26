using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public enum ReactionTypes
    {
		Like,
		Love,
		Surprised,
		Curious
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
