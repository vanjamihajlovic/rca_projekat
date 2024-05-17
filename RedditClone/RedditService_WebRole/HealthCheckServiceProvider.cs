using Contracts;

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
