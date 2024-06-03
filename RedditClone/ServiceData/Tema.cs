using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

/*
Tema:
 * ID
 - naslov
 - sadržaj
 - slika (ako korisnik želi da je postavi, nije obavezan deo posta)
 - vreme (ako baš želimo da zakomplikujemo)
 ~ lista sa komentarima
 ~ lista sa korisnicima koji su pretplaćeni (onda mi ne treba lista kod korisnika ?)
 ~ lista sa korisnicima koji su glasali na neku temu (kada korisnik uđe na post, proverava da li je njegov ID u listi, i onda menja lajk/dislajk na frontu)
*/

namespace ServiceData
{
    public class Tema : TableEntity
    {
        private string id;
        private string userId; // ko je kreirao
        private string naslov;
        private string sadrzaj;
        private string firstName;
        private string lastName;
        private string slika;   // url slike
        private List<string> komentari;
        private List<int> glasoviZa;      // za broj glasova, samo izvucemo Count
        private List<int> glasoviProtiv;  // isto to ovde
        private List<string> pretplaceniKorisnici;

        public List<string> PretplaceniKorisnici
        {
            get
            {
                if (pretplaceniKorisnici == null)
                    pretplaceniKorisnici = new List<string>();
                return pretplaceniKorisnici;
            }
            set { pretplaceniKorisnici = value; }
        }

        public string Id { get => id; set => id = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Naslov { get => naslov; set => naslov = value; }
        public string Sadrzaj { get => sadrzaj; set => sadrzaj = value; }
        public string Slika { get => slika; set => slika = value; }
        public List<string> Komentari { get => komentari; set => komentari = value; }
        public List<int> GlasoviZa { get => glasoviZa; set => glasoviZa = value; }
        public List<int> GlasoviProtiv { get => glasoviProtiv; set => glasoviProtiv = value; }
    //    public List<string> PretplaceniKorisnici { get => pretplaceniKorisnici; set => pretplaceniKorisnici = value; }
        public string UserId { get => userId; set => userId = value; }

        public Tema() { }

        public Tema(string id, string naslov, string sadrzaj, string userId,string firstName,string lastName) // string userId) //, string firstName, string lastName)
        {
            PartitionKey = "Tema";
            RowKey = id;

            Id = id;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Naslov = naslov;
            Sadrzaj = sadrzaj;
            Slika = "";
            Komentari = new List<string>();
            GlasoviZa = new List<int>();
            GlasoviProtiv = new List<int>();
            PretplaceniKorisnici = new List<string>();
        }
    }
}
