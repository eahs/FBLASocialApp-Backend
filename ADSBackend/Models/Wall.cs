using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class Wall
	{
		[Key]
		public int WallId { get; set; }
		public List<WallPost> Posts { get; set; }
	}
}
