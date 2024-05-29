using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceData;

namespace Contracts
{
	[ServiceContract]
	public interface IAdminEmail
	{
		[OperationContract]
		bool DodajMejlAdresu(AdminEmail ae);

		[OperationContract]
		bool ObrisiMejlAdresu(string adresa);

		[OperationContract]
		List<AdminEmail> DobaviSveMejlove();
	}
}
