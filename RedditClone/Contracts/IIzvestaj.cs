using ServiceData;
using System.Collections.Generic;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IIzvestaj
    {
        [OperationContract]
        bool SacuvajIzvestaj(Izvestaj i);

        [OperationContract]
        Izvestaj DobaviIzvestaj(string id);

        [OperationContract]
        double DobaviProsekZaPrethodniDan();

        [OperationContract]
        List<Izvestaj> DobaviIzvestajeZaPrethodniSat();
    }
}
