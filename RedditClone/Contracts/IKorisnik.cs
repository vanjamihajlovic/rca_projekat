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
    public interface IKorisnik
    {
        [OperationContract]
        bool DodajKorisnika(Korisnik k);

        [OperationContract]
        Korisnik DobaviKorisnika(string id);

        [OperationContract]
        bool IzmeniKorisnika(string id, Korisnik k);

        [OperationContract]
        bool ObrisiKorisnika(string id);

        [OperationContract]
        IQueryable<Korisnik> DobaviSve();
    }
}
