using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
