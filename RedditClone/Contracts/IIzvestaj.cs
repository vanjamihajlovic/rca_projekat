using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	[ServiceContract]
	public interface IIzvestaj
	{
		[OperationContract]
		bool SacuvajIzvestaj(Izvestaj i);

		[OperationContract]
		Izvestaj DobaviIzvestaj(string id);
	}
}
