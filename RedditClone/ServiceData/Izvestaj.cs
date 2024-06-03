using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ServiceData
{
    public class Izvestaj : TableEntity
    {
        public int Id { get; set; }
        public DateTime Vreme { get; set; }
        public string Sadrzaj { get; set; }

        public Izvestaj() {
            PartitionKey = "Izvestaj";
        }

        public Izvestaj(int id, DateTime vreme, string sadrzaj) : this()
        {
            PartitionKey = "Izvestaj";
            RowKey = id.ToString();

            Id = id;
            Vreme = vreme;
            Sadrzaj = sadrzaj;
        }
    }
}
