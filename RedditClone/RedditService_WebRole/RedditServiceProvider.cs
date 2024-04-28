using Contracts;
using Helpers;
using Microsoft.WindowsAzure.Storage.Queue;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using TableRepository;

// Metode: DodajKorisnika, IzmeniKorisnika, DodajPost, ObrisiPost, DodajKomentar, ObrisiKomentar, Upwote/downwote, PretplatiSeNaTemu
// Login/Register isto ovde, ili napraviti Auth klasu zasebnu?

// TODO negde treba dodati servis za slike (attachments) - kao poseban servis ili ?

namespace RedditService_WebRole
{
    public class RedditServiceProvider : IRedditService
    {
        CloudQueue queue = QueueHelper.GetQueueReference("CommentNotificationsQueue");
		// U funkciji za dodavanje komentara će se pozivati sledeće:
		// queue.AddMessage(new CloudQueueMessage(idKomentara));

		// Ovde se pravi TableRepositoryKomentar, TableRepositoryKorisnik, TableRepositoryTema

		// TODO promeniti, ovo je samo za sad
		public bool DodajKorisnika(Korisnik k)
        {
			TableRepositoryKorisnik tabela = new TableRepositoryKorisnik();
			Korisnik k1 = new Korisnik(123, "Test", "Jedan", "Dva", "Tri", "Cetiri", "1312", "mejl@gmail.com", "123");
            tabela.DodajKorisnika(k1);

            return true;
        }        
    }
}
