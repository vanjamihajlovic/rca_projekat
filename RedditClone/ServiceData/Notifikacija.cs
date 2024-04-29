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
		private int idKomentara;
		private DateTime vreme;
		private int brojPoslatihMejlova;

		public int IdKomentara { get => idKomentara; set => idKomentara = value; }
		public DateTime Vreme { get => vreme; set => vreme = value; }
		public int BrojPoslatihMejlova { get => brojPoslatihMejlova; set => brojPoslatihMejlova = value; }

		public Notifikacija() { }

		public Notifikacija(int idKomentara, DateTime vreme, int brojPoslatihMejlova)
		{
			PartitionKey = "Notifikacija";
			RowKey = idKomentara.ToString();

			IdKomentara = idKomentara;
			Vreme = vreme;
			BrojPoslatihMejlova = brojPoslatihMejlova;
		}
	}
}
