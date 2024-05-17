using ServiceData;
using System.ServiceModel;

// TODO napravi ga kako treba
// Metode: DodajKorisnika, IzmeniKorisnika, DodajPost, ObrisiPost, DodajKomentar, ObrisiKomentar, Upwote/downwote, PretplatiSeNaTemu

namespace Contracts
{
    [ServiceContract]
    public interface IRedditService
    {
        [OperationContract]
        bool DodajKorisnika(Korisnik k);
    }
}
