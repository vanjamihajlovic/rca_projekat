using Contracts;

namespace NotificationService_WorkerRole
{
    public class HealthCheckServiceProvider : IHealthCheck
    {
        public bool HealthCheck()
        {
            return true;
        }
    }
}
