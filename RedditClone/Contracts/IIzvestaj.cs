using ServiceData;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IIzvestaj
    {
        [OperationContract]
        bool SacuvajIzvestaj(Izvestaj i);

        [OperationContract]
        Izvestaj DobaviIzvestaj(string id);
    }
}
