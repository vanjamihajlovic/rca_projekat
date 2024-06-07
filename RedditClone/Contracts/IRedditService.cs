using ServiceData;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IRedditService
    {
        [OperationContract]
        bool DodajKorisnika(Korisnik k);
    }
}
