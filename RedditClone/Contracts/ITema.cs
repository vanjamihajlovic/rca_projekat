using ServiceData;
using System.Linq;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface ITema
    {
        [OperationContract]
        bool DodajTemu(Tema t);

        [OperationContract]
        Tema DobaviTemu(string id);

        [OperationContract]
        bool IzmeniTemu(string id, Tema t);

        [OperationContract]
        bool ObrisiTemu(string id);

        [OperationContract]
        IQueryable<Tema> DobaviSve();
    }
}
