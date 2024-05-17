using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

/*
Korisnik:
 * ID
 - ime
 - prezime
 - adresa
 - grad
 - država
 - broj telefona
 - email
 - lozinka
 - sličica
 ~ lista sa postovima (temama)
 ~ lista sa komentarima ?
 ~ lista sa temama na koje je pretplaćen ? 
*/

// TODO id da bude email adresa ? Tj. RowKey u tabeli

namespace ServiceData
{
    public class Korisnik : TableEntity
    {
        private string id;
        private string ime;
        private string prezime;
        private string adresa;
        private string grad;
        private string drzava;
        private string brTel;
        private string email;
        private string lozinka;
        private string slika;   // url slike
        private List<int> teme;

        public string Id { get => id; set => id = value; }
        public string Ime { get => ime; set => ime = value; }
        public string Prezime { get => prezime; set => prezime = value; }
        public string Adresa { get => adresa; set => adresa = value; }
        public string Grad { get => grad; set => grad = value; }
        public string Drzava { get => drzava; set => drzava = value; }
        public string BrTel { get => brTel; set => brTel = value; }
        public string Email { get => email; set => email = value; }
        public string Lozinka { get => lozinka; set => lozinka = value; }
        public string Slika { get => slika; set => slika = value; }
        public List<int> Teme { get => teme; set => teme = value; }

        public Korisnik() { }

        public Korisnik(string id, string ime, string prezime, string adresa, string grad, string drzava, string brTel, string email, string lozinka)
        {
            PartitionKey = "Korisnik";
            RowKey = email;

            Id = id;
            Ime = ime;
            Prezime = prezime;
            Adresa = adresa;
            Grad = grad;
            Drzava = drzava;
            BrTel = brTel;
            Email = email;
            Lozinka = lozinka;  // heširati ?
            Slika = "";         // default - nema sliku, ili staviti neki placeholder
            Teme = new List<int>();
        }
    }
}
