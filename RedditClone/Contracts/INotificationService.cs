using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	[ServiceContract]
	public interface INotificationService
	{
        [OperationContract]
        Task PosaljiMejl(string korisnikEmail, string tekstKomentara, string autorKomentara, DateTime vreme);

       

    }
}
