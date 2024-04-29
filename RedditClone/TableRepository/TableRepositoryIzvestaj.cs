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
	}
}
