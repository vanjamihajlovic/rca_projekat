using Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
        
        public async Task<IQueryable<Tema>> DobaviSvePaginirano(int page, int pageSize, string sortBy, string searchCriteria)
        {
            try
            {
                var tableQuery = new TableQuery<Tema>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Tema"));
                var results = new List<Tema>();

                TableContinuationToken continuationToken = null;
                do
                {
                    var currentSegment = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                    continuationToken = currentSegment.ContinuationToken;
                    results.AddRange(currentSegment.Results);
                } while (continuationToken != null);

                var sortedResults = results.AsQueryable();

                // Pretraga po naslovu
                if (searchCriteria != "")
                {
                    sortedResults = sortedResults.Where(t => t.Naslov.Contains(searchCriteria));
                }

                // Sortiranje
                if (sortBy.ToLower().Equals("asc"))
                {
                    sortedResults = sortedResults.OrderBy(post => post.Naslov);
                }
                else
                {
                    sortedResults = sortedResults.OrderByDescending(post => post.Naslov);
                }

                // Paginacija
                return sortedResults.Skip((page - 1) * pageSize).Take(pageSize).AsQueryable();
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error fetching paginated results: {ex.Message}");
                throw;
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
                Tema tmp = (from g in table.CreateQuery<Tema>() where g.PartitionKey == "Tema" && g.RowKey == id select g).FirstOrDefault();

                if (tmp == null)
                {
                    Trace.WriteLine("Post with id " + id + " does not exist.");
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

        public async Task<bool> UpdateVoteCount(string rowKey, bool isIncrement, bool isUpvote)
        {
            try
            {
                var retrieveOperation = TableOperation.Retrieve<Tema>("Tema", rowKey);
                var retrievedResult = await table.ExecuteAsync(retrieveOperation);
                var tema = retrievedResult.Result as Tema;

                if (tema == null)
                {
                    Trace.WriteLine($"Entity with RowKey {rowKey} not found.");
                    return false;
                }
                if (isUpvote)
                {
                    tema.GlasoviZa += isIncrement ? 1 : -1;
                } else
                {
                    tema.GlasoviProtiv += isIncrement ? 1 : -1;
                }
                var updateOperation = TableOperation.InsertOrReplace(tema);

                await table.ExecuteAsync(updateOperation);

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error updating GlasoviZa: {ex.Message}");
                return false;
            }
        }

        public IQueryable<Tema> PretraziTeme(string searchTerm)
        {
            try
            {
                // Uzimanje svih tema iz tabele sa odgovarajućom PartitionKey i filtriranje po naslovu
                var results = from g in table.CreateQuery<Tema>()
                              where g.PartitionKey == "Tema" && g.Naslov.ToLower().Contains(searchTerm.ToLower())
                              select g;

                return results;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error while searching topics: {ex.Message}");
                throw;
            }
        }
    }
}
