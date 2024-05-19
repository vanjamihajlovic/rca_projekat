using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditService_WebRole.Models
{
    public class SubscriptionRequest
    { 
        public string UserId { get; set; }
        public string PostId { get; set; }
    }
}