using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditService_WebRole.Models
{
	public class Vote
	{
		public string VoteId { get; set; }
		public string UserId { get; set; }
		public string PostId { get; set; }
		public bool IsUpvote { get; set; }
		public DateTime VotedAt { get; set; }
	}
}