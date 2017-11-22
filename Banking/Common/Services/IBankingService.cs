using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    [ServiceContract]
    public interface IBankingService
    {
        [OperationContract]
        bool Check(Request request, int pin);
    }
}
