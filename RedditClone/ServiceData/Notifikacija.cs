using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceData
{
	public class Notifikacija : TableEntity
	{
		private int id;
		private DateTime vreme;
		private int brojPoslatihMejlova;

		public int Id { get => id; set => id = value; }
		public DateTime Vreme { get => vreme; set => vreme = value; }
		public int BrojPoslatihMejlova { get => brojPoslatihMejlova; set => brojPoslatihMejlova = value; }

		public Notifikacija() { }

		public Notifikacija(int id, DateTime vreme, int brojPoslatihMejlova)
		{
			PartitionKey = "Notifikacija";
			RowKey = id.ToString();

			Id = id;
			Vreme = vreme;
			BrojPoslatihMejlova = brojPoslatihMejlova;
		}
	}
}
