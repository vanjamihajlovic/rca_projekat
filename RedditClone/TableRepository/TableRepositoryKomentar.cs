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
    public class TableRepositoryKomentar : IKomentar
    {
        private CloudStorageAccount storageAccount;
        private CloudTable table;

        public TableRepositoryKomentar()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            table = tableClient.GetTableReference("RedditKomentarTabela");
            table.CreateIfNotExists();
        }

        public Komentar DobaviKomentar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new Komentar();

            try
            {
                var kom = (from g in table.CreateQuery<Komentar>() where g.PartitionKey == "Komentar" && g.RowKey == id select g).FirstOrDefault();
                return kom ?? new Komentar();
            }
            catch (Exception)
            {
                return new Komentar();
            }
        }

        public IQueryable<Komentar> DobaviSve()
        {
            try
            {
                var results = from g in table.CreateQuery<Komentar>()
                              where g.PartitionKey == "Komentar"
                              select g;
                return results;
            }
            catch (Exception)
            {
                return Enumerable.Empty<Komentar>().AsQueryable();
            }
        }

        public bool DodajKomentar(Komentar k)
        {
            if (k == null) return false;

            try
            {
                TableOperation insertOperation = TableOperation.Insert(k);
                table.Execute(insertOperation);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        public bool IzmeniKomentar(string id, Komentar k)
        {
            if (k == null || string.IsNullOrEmpty(id)) return false;

            try
            {
                TableOperation updateOperation = TableOperation.InsertOrReplace(k);
                table.Execute(updateOperation);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        public bool ObrisiKomentar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            try
            {
                Komentar tmp = (from g in table.CreateQuery<Komentar>() where g.PartitionKey == "Komentar" && g.RowKey == id select g).FirstOrDefault();

                if (tmp == null)
                {
                    Trace.WriteLine("Comment with id " + id + " does not exist.");
                    return false;
                }

                TableOperation deleteOperation = TableOperation.Delete(tmp);
                table.Execute(deleteOperation);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
