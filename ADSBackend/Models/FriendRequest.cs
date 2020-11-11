using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public enum FriendRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class FriendRequest
    {
        [Key]
        public int FriendRequestId { get; set; }

        // Member making the request
        public int MemberId { get; set; }   
        public Member Member { get; set; }

        // Friend
        public int FriendId { get; set; }
        public Member Friend { get; set; }

        public DateTime RequestIssuedAt { get; set; }
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
    }
}
