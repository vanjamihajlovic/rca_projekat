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
    public class TableRepositoryKorisnik : IKorisnik
    {
        private CloudStorageAccount storageAccount;
        private CloudTable table;

        public TableRepositoryKorisnik()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            table = tableClient.GetTableReference("Korisnici");
            table.CreateIfNotExists();
        }


        public Korisnik DobaviKorisnika(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new Korisnik();

            try
            {
                var kor = (from g in table.CreateQuery<Korisnik>() where g.PartitionKey == "Korisnik" && g.RowKey == id select g).FirstOrDefault();
                return kor ?? new Korisnik();
            }
            catch (Exception)
            {
                return new Korisnik();
            }
        }

        public IQueryable<Korisnik> DobaviSve()
        {
            try
            {
                var results = from g in table.CreateQuery<Korisnik>()
                              where g.PartitionKey == "Korisnik"
                              select g;
                return results;
            }
            catch (Exception)
            {
                return Enumerable.Empty<Korisnik>().AsQueryable();
            }
        }

        public bool DodajKorisnika(Korisnik k)
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

        public bool IzmeniKorisnika(string id, Korisnik k)
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

        public bool ObrisiKorisnika(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            try
            {
                Korisnik tmp = (from g in table.CreateQuery<Korisnik>() where g.PartitionKey == "Korisnik" && g.RowKey == id select g).FirstOrDefault();

                if (tmp == null)
                {
                    Trace.WriteLine("User with id " + id + " does not exist.");
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
