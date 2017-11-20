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
        /// <summary>
        /// Opening account
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        [OperationContract]
        int OpenAccount(string accountName);

        /// <summary>
        /// Loaning money amouunt
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="amount">Amount of money for loan</param>
        /// <returns></returns>
        [OperationContract]
        bool RaiseALoan(string accountName, int amount, int pin);

        /// <summary>
        /// Payment method
        /// </summary>
        /// <param name="isPayment">If is true than is +payment else is -payment</param>
        /// <param name="accountName"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [OperationContract]
        bool Payment(bool isPayment, string accountName, int amount, int pin);
        
    }
}
