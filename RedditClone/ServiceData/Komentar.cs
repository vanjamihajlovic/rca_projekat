using Microsoft.WindowsAzure.Storage.Table;
using System;

// Ako TableEntity ne radi, ručno preko NuGet instalirati:
// Microsoft.WindowsAzure.Storage.Table v8.0.1
// Microsoft.WindowsAzure.ConfigurationManager v2.0.3

/*
Komentar:
 * ID
 - idTeme
 - naslov
 - autor
 - sadržaj
 - vreme (ako baš želimo da zakomplikujemo)
*/

namespace ServiceData
{
    public class Komentar : TableEntity
    {
        private int id;
        private string idTeme;
        private string autor;
        private string sadrzaj;

        public int Id { get => id; set => id = value; }
        public string IdTeme { get => idTeme; set => idTeme = value; }
        public string Autor { get => autor; set => autor = value; }
        public string Sadrzaj { get => sadrzaj; set => sadrzaj = value; }

        public Komentar() { }

        public Komentar(string idTeme, string autor, string sadrzaj)
        {
            PartitionKey = "Komentar";
            RowKey = Guid.NewGuid().ToString();

            IdTeme = idTeme;
            Sadrzaj = sadrzaj;
            Autor = autor;
        }
    }
}
