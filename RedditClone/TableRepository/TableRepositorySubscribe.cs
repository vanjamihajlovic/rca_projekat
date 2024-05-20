﻿using Contracts;
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

        public IQueryable<Subscribe> DobaviSve()
        {
            try
            {
                var results = from g in table.CreateQuery<Subscribe>()
                              where g.PartitionKey == "Subscribe"
                              select g;
                return results;
            }
            catch (Exception)
            {
                return Enumerable.Empty<Subscribe>().AsQueryable();
            }
        }

        public Subscribe DobaviSubscribe(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new Subscribe();

            try
            {
                var subscribe = (from g in table.CreateQuery<Subscribe>() where g.PartitionKey == "Subscribe" && g.RowKey == id select g).FirstOrDefault();
                return subscribe ?? new Subscribe();
            }
            catch (Exception)
            {
                return new Subscribe();
            }
        }

        //dodati metoddu koja prima id teme i vraca suba za tu temu
        public List<Subscribe> DobaviSvePrijavljene(string post)
        {
            var results = from g in table.CreateQuery<Subscribe>()
                          where g.PartitionKey == "Subscribe" && g.RowKey == post
                          select g;
            return results.ToList<Subscribe>();
        }

        public bool SubscribeToPost(Subscribe s)
        {
            if (s == null) return false;

            try
            {
                // Kreiranje operacije za insert ili replace entiteta u tabelu
                TableOperation insertOperation = TableOperation.Insert(s); // ili INSERT ?
                table.Execute(insertOperation);
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
