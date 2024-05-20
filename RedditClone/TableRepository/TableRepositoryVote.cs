﻿using Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace TableRepository
{
	public class TableRepositoryVote : IVote
	{
		private CloudStorageAccount storageAccount;
		private CloudTable table;

		public TableRepositoryVote()
		{
			storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
			CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
			table = tableClient.GetTableReference("RedditVotesTable");
			table.CreateIfNotExists();
		}

		public bool DodajGlas(Vote glas)
		{
			if (glas == null) return false;

			try
			{
				TableOperation insertOperation = TableOperation.Insert(glas);
				table.Execute(insertOperation);
				return true;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				return false;
			}
		}

		public bool AzurirajGlas(string idGlasa, Vote glas)
		{
			if (glas == null || idGlasa == null) return false;

			try
			{
				glas.VoteId = idGlasa; // Postavi ID glasa
				TableOperation updateOperation = TableOperation.Replace(glas);
				table.Execute(updateOperation);
				return true;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				return false;
			}
		}

        public bool ObrisiGlas(string voteId)
        {
            if (string.IsNullOrEmpty(voteId))
                return false;

            try
            {
                // Koristimo LINQ upit za dohvaćanje glasa
                Vote tmp = (from g in table.CreateQuery<Vote>()
                            where g.PartitionKey == "Vote" && g.RowKey == voteId
                            select g).FirstOrDefault();

                if (tmp == null)
                {
                    Trace.WriteLine($"Vote with ID {voteId} does not exist.");
                    return false;
                }

                // Kreiramo operaciju za brisanje
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


        public Vote DobaviGlas(string idGlasa)
		{
			if (idGlasa == null)
				return null;

			try
			{
				TableOperation retrieveOperation = TableOperation.Retrieve<Vote>("Vote", idGlasa);
				TableResult retrievedResult = table.Execute(retrieveOperation);
				return (Vote)retrievedResult.Result;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				return null;
			}
		}

		public List<Vote> DobaviSveGlasoveZaPost(string idPosta)
		{
			try
			{
				var query = new TableQuery<Vote>().Where(TableQuery.GenerateFilterCondition("PostId", QueryComparisons.Equal, idPosta));
				return table.ExecuteQuery(query).ToList();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				return new List<Vote>();
			}
		}

		public List<Vote> DobaviSveGlasoveZaKorisnika(string idKorisnika)
		{
			try
			{
				var query = new TableQuery<Vote>().Where(TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, idKorisnika));
				return table.ExecuteQuery(query).ToList();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				return new List<Vote>();
			}
		}

      

    }
}
