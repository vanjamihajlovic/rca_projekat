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
    public class TableRepositoryTema : ITema
    {
        private CloudStorageAccount storageAccount;
        private CloudTable table;

        public TableRepositoryTema()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            table = tableClient.GetTableReference("RedditTemaTabela");
            table.CreateIfNotExists();
        }

        public IQueryable<Tema> DobaviSve()
        {
            try
            {
                var results = from g in table.CreateQuery<Tema>()
                              where g.PartitionKey == "Tema"
                              select g;
                return results;
            }
            catch (Exception)
            {
                return Enumerable.Empty<Tema>().AsQueryable();
            }
        }

        public Tema DobaviTemu(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new Tema();

            try
            {
                var tema = (from g in table.CreateQuery<Tema>() where g.PartitionKey == "Tema" && g.RowKey == id select g).FirstOrDefault();
                return tema ?? new Tema();
            }
            catch (Exception)
            {
                return new Tema();
            }
        }

        public bool DodajTemu(Tema t)
        {
            if (t == null) return false;

            try
            {
                TableOperation insertOperation = TableOperation.Insert(t);
                table.Execute(insertOperation);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        public bool IzmeniTemu(string id, Tema t)
        {
            if (t == null || string.IsNullOrEmpty(id)) return false;

            try
            {
                TableOperation updateOperation = TableOperation.InsertOrReplace(t);
                table.Execute(updateOperation);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        public bool ObrisiTemu(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            try
            {
                // select
                Tema tmp = (from g in table.CreateQuery<Tema>() where g.PartitionKey == "Tema" && g.RowKey == id select g).FirstOrDefault();

                if (tmp == null)
                {
                    Trace.WriteLine("Post with id " + id + " does not exist.");
                    return false;
                }

                // delete
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
