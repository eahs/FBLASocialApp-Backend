using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class PostReaction
	{
		public int PostId { get; set; }
		public Post Post { get; set; }
		public int ReactionId { get; set; }
		public Reaction Reaction { get; set; }
	}
}
