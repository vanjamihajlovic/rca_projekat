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
	public interface IVote
	{
		// Dodavanje novog glasa
		[OperationContract]
		bool DodajGlas(Vote glas);

		// Ažuriranje postojećeg glasa
		[OperationContract]
		bool AzurirajGlas(string idGlasa, Vote glas);

		// Brisanje glasa
		[OperationContract]
		bool ObrisiGlas(string idGlasa);

		// Dobavljanje glasa po ID
		[OperationContract]
		Vote DobaviGlas(string idGlasa);

		// Dobavljanje svih glasova za određeni post (temu ili komentar)
		[OperationContract]
		List<Vote> DobaviSveGlasoveZaPost(string idPosta);

		// Dobavljanje svih glasova koje je dao određeni korisnik
		[OperationContract]
		List<Vote> DobaviSveGlasoveZaKorisnika(string idKorisnika);
	}
}
