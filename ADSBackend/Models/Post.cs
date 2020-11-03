using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public enum PrivacyLevel
    {
		Public = 0,
		FriendsOnly = 1,
		Private = 2
    };

	public class Post
	{
		[Key]
		public int PostId { get; set; }
		[Required]
		public int AuthorId { get; set; }
		public Member Author { get; set; }
		[Required]
		public string Title { get; set; }
		public string Body { get; set; }

		public List<PostPhoto> Images { get; set; }

		[Required]
		public bool IsMachinePost { get; set; } = false;
		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime EditedAt { get; set; }
		[Required]
		public bool IsDeleted { get; set; } = false;
		public List<PostReaction> Reactions { get; set; }
		public List<PostComment> Comments { get; set; }
		[Required]
		public PrivacyLevel PrivacyLevel { get; set; } = PrivacyLevel.Public;
		public int FavoriteCount { get; set; }
		[Required]
		public bool IsFeatured { get; set; } = false;
		[NotMapped] public bool IsFavorite { get; set; } = false;

	}
}
