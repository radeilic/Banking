using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    [ServiceContract]
    public interface IUserServices
    {
        [OperationContract]
        bool OpenAccount(string accountName);

        [OperationContract]
        bool RaiseALoan(string accountName, int amount);

        [OperationContract]
        bool Payment(bool isPayment, string accountName, int amount);
        
    }
}
