using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceData
{
	public class Izvestaj : TableEntity
	{
		private int id;
		private DateTime vreme;
		private string sadrzaj;

		public int Id { get => id; set => id = value; }
		public DateTime Vreme { get => vreme; set => vreme = value; }
		public string Sadrzaj { get => sadrzaj; set => sadrzaj = value; }

		public Izvestaj() { }

		public Izvestaj(int id, DateTime vreme, string sadrzaj)
		{
			PartitionKey = "Izvestaj";
			RowKey = id.ToString();

			Id = id;
			Vreme = vreme;
			Sadrzaj = sadrzaj;
		}
	}
}
