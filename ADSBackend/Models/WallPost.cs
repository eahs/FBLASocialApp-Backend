using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class WallPost
	{
		public int PostId { get; set; }
		public Post Post { get; set; }
		public int WallId { get; set; }
		public Wall Wall { get; set; }
	}
}
