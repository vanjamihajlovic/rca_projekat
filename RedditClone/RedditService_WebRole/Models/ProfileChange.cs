using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditService_WebRole.Models
{
    public class ProfileChange : Register
    {
        public string OldEmail { get; set; }
    }
}