using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface INotificationService
    {
        [OperationContract]
        Task PosaljiMejl(string korisnikEmail, string tekstKomentara, string autorKomentara, DateTime vreme, string naslovTeme);
    }
}
