using Common;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService
{
    public class AdminServices : IAdminServices
    {
        public bool CheckRequest()
        {
            Console.WriteLine("CheckRequest called.");
            return true;
        }

        public bool Init()
        {
            
            Database.accounts = new List<Account>();
            Database.accountsRequests = new Queue<Request>();
            Database.loansRequests = new Queue<Request>();
            Database.paymentRequests = new Queue<Request>();
            
            return true;
        }
    }
}
