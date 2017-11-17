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
            
            Database.accounts = new Dictionary<string, Account>();
            Database.accountsRequests = new List<Request>();
            Database.loansRequests = new List<Request>();
            Database.paymentRequests = new List<Request>();
            
            return true;
        }
    }
}
