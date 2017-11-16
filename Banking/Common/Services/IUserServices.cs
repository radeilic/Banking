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
        bool OpenAccount();

        [OperationContract]
        bool RaiseALoan();

        [OperationContract]
        bool Payment();
    }
}
