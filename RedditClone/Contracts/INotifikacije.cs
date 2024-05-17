using ServiceData;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface INotifikacije
    {
        [OperationContract]
        bool SacuvajNotifikaciju(Notifikacija n);
    }
}
