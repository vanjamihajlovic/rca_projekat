using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IHealthCheck
    {
        [OperationContract]
        bool HealthCheck();
    }
}
