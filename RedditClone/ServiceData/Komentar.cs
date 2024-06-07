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
        private string idTeme;
        private string autor;
        private string sadrzaj;
        private string authorName;

        public string IdTeme { get => idTeme; set => idTeme = value; }
        public string Autor { get => autor; set => autor = value; }
        public string Sadrzaj { get => sadrzaj; set => sadrzaj = value; }
        public string AuthorName { get => authorName; set => authorName = value; }

        public Komentar() {
            PartitionKey = "Komentar";
        }

        public Komentar(string idTeme, string autor, string sadrzaj, string authorName) : this()
        {
            RowKey = Guid.NewGuid().ToString();

            IdTeme = idTeme;
            Sadrzaj = sadrzaj;
            Autor = autor;
            AuthorName = authorName;
        }
    }
}
