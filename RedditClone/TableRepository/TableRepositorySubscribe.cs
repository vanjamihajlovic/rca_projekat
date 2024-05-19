using Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableRepository
{
    public class TableRepositorySubscribe : ISubscribe
    {
        private CloudStorageAccount storageAccount;
        private CloudTable table;
       

        public TableRepositorySubscribe()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            table = tableClient.GetTableReference("RedditSubscribeTabela");
            table.CreateIfNotExists();
        }

        public bool SubscribeToPost(Subscribe s)
        {
            if (s == null) return false;

            try
            {
                // Kreiranje operacije za insert ili replace entiteta u tabelu
                TableOperation insertOperation = TableOperation.InsertOrReplace(s); // ili INSERT ?
                table.Execute(insertOperation);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UnsubscribeFromPost(Subscribe s)
        {
            if (s == null)
                return false;
            try
            {
                // select
                Subscribe tmp = (from g in table.CreateQuery<Subscribe>() where g.PartitionKey == "Subscribe" && g.RowKey == s.PostId select g).FirstOrDefault();

                if (tmp == null)
                {
                    Trace.WriteLine("Post with id " + s.PostId + " does not exist.");
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
