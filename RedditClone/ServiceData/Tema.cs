﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private int id;
        private string naslov;
        private string sadrzaj;
        private string slika;   // url slike
        private List<int> komentari;
        private List<int> glasoviZa;      // za broj glasova, samo izvucemo Count
        private List<int> glasoviProtiv;  // isto to ovde
        private List<int> pretplaceniKorisnici;

        public int Id { get => id; set => id = value; }
        public string Naslov { get => naslov; set => naslov = value; }
        public string Sadrzaj { get => sadrzaj; set => sadrzaj = value; }
        public string Slika { get => slika; set => slika = value; }
        public List<int> Komentari { get => komentari; set => komentari = value; }
        public List<int> GlasoviZa { get => glasoviZa; set => glasoviZa = value; }
        public List<int> GlasoviProtiv { get => glasoviProtiv; set => glasoviProtiv = value; }
        public List<int> PretplaceniKorisnici { get => pretplaceniKorisnici; set => pretplaceniKorisnici = value; }

        public Tema() { }

        public Tema(int id, string naslov, string sadrzaj)
        {
            PartitionKey = "Tema";
            RowKey = id.ToString();

            Id = id;
            Naslov = naslov;
            Sadrzaj = sadrzaj;
            Slika = "";
            Komentari = new List<int>();
            GlasoviZa = new List<int>();
            GlasoviProtiv = new List<int>();
            PretplaceniKorisnici = new List<int>();
        }
    }
}
