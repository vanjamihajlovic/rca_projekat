using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO napravi ga kako treba
// Metode: DodajKorisnika, IzmeniKorisnika, DodajPost, ObrisiPost, DodajKomentar, ObrisiKomentar, Upwote/downwote, PretplatiSeNaTemu

namespace Contracts
{
    public interface IRedditService
    {
        bool DodajKorisnika(Korisnik k);
    }
}
