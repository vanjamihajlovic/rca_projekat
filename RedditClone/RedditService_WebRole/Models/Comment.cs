using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditService_WebRole.Models
{
    public class Comment
    {
        public string TopicId { get; set; }
        public string Text { get; set; }
    }
}