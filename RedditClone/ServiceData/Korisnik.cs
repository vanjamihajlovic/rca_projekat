using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace ServiceData
{
    public class Korisnik : TableEntity
    {
        public string Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Adresa { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }
        public string BrTel { get; set; }
        public string Email { get; set; }
        public string Lozinka { get; set; }
        public string Slika { get; set; }
        public List<int> Teme { get; set; }

        public Korisnik()
        {
            PartitionKey = "Korisnik";
        }

        public Korisnik(string id, string ime, string prezime, string adresa, string grad, string drzava, string brTel, string email, string lozinka)
            : this()
        {
            RowKey = email;
            Id = id;
            Ime = ime;
            Prezime = prezime;
            Adresa = adresa;
            Grad = grad;
            Drzava = drzava;
            BrTel = brTel;
            Email = email;
            Lozinka = lozinka;
            Slika = "";  // default - nema sliku, ili staviti neki placeholder
            Teme = new List<int>();
        }
    }
}
