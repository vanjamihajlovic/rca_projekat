using Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TableRepository
{
    public class TableRepositoryIzvestaj : IIzvestaj
    {
        private CloudStorageAccount storageAccount;
        private CloudTable table;

        public TableRepositoryIzvestaj()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            table = tableClient.GetTableReference("IzvestajTabela");
            table.CreateIfNotExists();
        }

        public bool SacuvajIzvestaj(Izvestaj i)
        {
            if (i == null) return false;

            try
            {
                TableOperation insertOperation = TableOperation.Insert(i);
                table.Execute(insertOperation);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        public Izvestaj DobaviIzvestaj(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new Izvestaj();

            try
            {
                var izv = (from g in table.CreateQuery<Izvestaj>() where g.PartitionKey == "Izvestaj" && g.RowKey == id select g).FirstOrDefault();
                return izv ?? new Izvestaj();
            }
            catch (Exception)
            {
                return new Izvestaj();
            }
        }
        
        // Za prethodnih sat vremena
        // 20 izveštaja u minuti - 1200 u satu
        // Biće 4800 u satu jer imamo 3+1 servis
        public List<Izvestaj> DobaviIzvestajeZaPrethodniSat()
        {
            DateTime odKada = DateTime.Now.Subtract(TimeSpan.FromHours(1));

            try
            {
                var izv = (from g in table.CreateQuery<Izvestaj>() where g.PartitionKey == "Izvestaj" && g.Timestamp > odKada select g);
                return izv.ToList<Izvestaj>();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new List<Izvestaj>();
            }
        }

        public double DobaviProsekZaPrethodniDan()
        {
            DateTime odKada = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            try
            {
                var izv = (from g in table.CreateQuery<Izvestaj>() where g.PartitionKey == "Izvestaj" && g.Timestamp > odKada select g);
                int suma = 0;
                int broj = 0;
                foreach (var i in izv)
                {
                    if (i.Sadrzaj.ToString() == "RedditService OK")
                    {
                        suma++;
                        broj++;
                    }
                    else if (i.Sadrzaj.ToString() == "RedditService NOT OK")
                    {
                        broj++;
                    }
                }

                double prosek = suma / broj;
                return prosek;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
