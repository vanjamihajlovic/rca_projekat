using ServiceData;
using System.Linq;
using System.ServiceModel;

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
