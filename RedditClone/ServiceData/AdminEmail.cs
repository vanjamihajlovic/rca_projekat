using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceData
{
	public class AdminEmail : TableEntity
	{
		private string emailAdresa;

		public string EmailAdresa { get => emailAdresa; set => emailAdresa = value; }

		public AdminEmail() { }

		public AdminEmail(string adresa)
		{
			PartitionKey = "AdminEmail";
			RowKey = Guid.NewGuid().ToString();

			EmailAdresa = adresa;
		}
	}
}
