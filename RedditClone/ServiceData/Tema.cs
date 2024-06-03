using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ServiceData
{
    public class Tema : TableEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Naslov { get; set; }
        public string Sadrzaj { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Slika { get; set; }
        public List<Komentar> Komentari { get; set; } = new List<Komentar>();
        public List<int> GlasoviZa { get; set; } = new List<int>();
        public List<int> GlasoviProtiv { get; set; } = new List<int>();
        public List<string> PretplaceniKorisnici { get; set; } = new List<string>();

        public Tema() {
            PartitionKey = "Tema";
        }

        public Tema(string id, string naslov, string sadrzaj, string userId, string firstName, string lastName) : this()
        {
            RowKey = id;
            Id = id;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Naslov = naslov;
            Sadrzaj = sadrzaj;
        }
    }
}
