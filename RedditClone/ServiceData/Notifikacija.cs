using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ServiceData
{
    public class Notifikacija : TableEntity
    {
        public int IdKomentara { get; set; }
        public DateTime Vreme { get; set; }
        public int BrojPoslatihMejlova { get; set; }

        public Notifikacija() { 
            PartitionKey = "Notifikacija";
        }

        public Notifikacija(int idKomentara, DateTime vreme, int brojPoslatihMejlova) : this()
        {
            RowKey = idKomentara.ToString();

            IdKomentara = idKomentara;
            Vreme = vreme;
            BrojPoslatihMejlova = brojPoslatihMejlova;
        }
    }
}
