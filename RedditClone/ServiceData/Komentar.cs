using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		private int idTeme;
        private string naslov;
        private string autor;
        private string sadrzaj;

        public int Id { get => id; set => id = value; }
		public int IdTeme { get => idTeme; set => idTeme = value; }
		public string Naslov { get => naslov; set => naslov = value; }
        public string Autor { get => autor; set => autor = value; }
        public string Sadrzaj { get => sadrzaj; set => sadrzaj = value; }

		public Komentar() { }

        public Komentar(int id, int idTeme, string naslov, string autor, string sadrzaj)
        {
            PartitionKey = "Komentar";
            RowKey = id.ToString();

            Id = id;
			IdTeme = idTeme;
            Naslov = naslov;
            Sadrzaj = sadrzaj;
            Autor = autor;
        }
    }
}
