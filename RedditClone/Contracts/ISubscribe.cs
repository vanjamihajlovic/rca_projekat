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
    public interface ISubscribe
    {
        [OperationContract]
        bool SubscribeToPost(Subscribe s);

        [OperationContract]
        bool UnsubscribeFromPost(Subscribe s);

        [OperationContract]
        IQueryable<Subscribe> DobaviSve();

        [OperationContract]
        Subscribe DobaviSubscribe(string id);
    }
}
