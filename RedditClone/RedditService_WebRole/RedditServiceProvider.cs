using Contracts;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// I za ovo vrv treba interfejs u Common
// Metode: DodajKorisnika, IzmeniKorisnika, DodajPost, ObrisiPost, DodajKomentar, ObrisiKomentar, Upwote/downwote, PretplatiSeNaTemu
// Login/Register isto ovde, ili napraviti Auth klasu zasebnu?

// TODO negde treba dodati servis za slike (attachments) - kao poseban servis ili ?

namespace RedditService_WebRole
{
    public class RedditServiceProvider : IRedditService
    {
        public bool DodajKorisnika(Korisnik k)
        {
            throw new NotImplementedException();
        }
    }
}
