using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class UserServices : IUserServices
    {
        public bool OpenAccount(string accountName)
        {
            Console.WriteLine("OpenAccount called.");
            return true;
        }

        public bool Payment(bool isPayment, string accountName, int amount)
        {
            Console.WriteLine("Payment called.");
            return true;

        }

        public bool RaiseALoan(string accountName, int amount)
        {
            Console.WriteLine("RaiseALoan called.");
            return true;
        }
    }
}
