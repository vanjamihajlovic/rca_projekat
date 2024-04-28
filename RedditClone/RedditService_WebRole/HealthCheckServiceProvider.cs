using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditService_WebRole
{
    public class HealthCheckServiceProvider : IHealthCheck
    {
        public bool HealthCheck()
        {
            return true;
        }
    }
}
