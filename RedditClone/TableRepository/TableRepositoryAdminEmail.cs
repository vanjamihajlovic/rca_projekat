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
	public class TableRepositoryAdminEmail : IAdminEmail
	{
		private CloudStorageAccount storageAccount;
		private CloudTable table;

		public TableRepositoryAdminEmail()
		{
			storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
			CloudTableClient tableClient = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
			table = tableClient.GetTableReference("AdminEmailTabela");
			table.CreateIfNotExists();
		}
		
		public List<AdminEmail> DobaviSveMejlove()
		{
			try
			{
				var results = from g in table.CreateQuery<AdminEmail>()
							  where g.PartitionKey == "AdminEmail"
							  select g;
				return results.ToList();
			}
			catch (Exception)
			{
				return new List<AdminEmail>();
			}
		}

		public bool ObrisiMejlAdresu(string adresa)
		{
			if (adresa.Equals("")) return false;

			try
			{
				AdminEmail tmp = (from g in table.CreateQuery<AdminEmail>() where g.PartitionKey == "AdminEmail" && g.EmailAdresa == adresa select g).FirstOrDefault();

				if (tmp == null)
				{
					Trace.WriteLine("This email address does not exist.");
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

		public bool DodajMejlAdresu(AdminEmail ae)
		{
			if (ae == null) return false;

			try
			{
				AdminEmail tmp = (from g in table.CreateQuery<AdminEmail>() where g.PartitionKey == "AdminEmail" && g.EmailAdresa == ae.EmailAdresa select g).FirstOrDefault();
				if (tmp != null)
				{
					return false;
				}

				TableOperation insertOperation = TableOperation.Insert(ae);
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
