using Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceData;
using System;
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
    }
}
