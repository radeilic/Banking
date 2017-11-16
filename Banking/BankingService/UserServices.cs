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
        public bool OpenAccount()
        {
            Console.WriteLine("OpenAccount called.");
            return true;
        }

        public bool Payment()
        {
            Console.WriteLine("Payment called.");
            return true;

        }

        public bool RaiseALoan()
        {
            Console.WriteLine("RaiseALoan called.");
            return true;
        }
    }
}
