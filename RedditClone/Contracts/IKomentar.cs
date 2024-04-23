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
    public interface IKomentar
    {
        [OperationContract]
        bool DodajKomentar(Komentar k);

        [OperationContract]
        Komentar DobaviKomentar(string id);

        [OperationContract]
        bool IzmeniKomentar(string id, Komentar k);

        [OperationContract]
        bool ObrisiKomentar(string id);

        [OperationContract]
        IQueryable<Komentar> DobaviSve();
    }
}
