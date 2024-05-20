using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditService_WebRole.Models
{
	public class Voting
	{
		public string UserId { get; set; }
		public string PostId { get; set; }
	}
}