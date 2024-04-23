using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
