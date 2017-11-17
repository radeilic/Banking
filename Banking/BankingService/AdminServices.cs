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
            lock (Database.accountRequestsLock)
            {
                for (int i = 0;i < Database.accountsRequests.Count; i++)
                {
                    TimeSpan time = DateTime.Now - Database.accountsRequests[i].TimeOfCreation;
                    double milliseconds = time.Milliseconds;
                    if (milliseconds > 500)
                    {
                        Database.accountsRequests.RemoveAt(i);
                        i--;
                    }
                }
            }

            lock (Database.loansRequestsLock)
            {
                for (int i = 0; i < Database.accountsRequests.Count; i++)
                {
                    TimeSpan time = DateTime.Now - Database.accountsRequests[i].TimeOfCreation;
                    double milliseconds = time.Milliseconds;
                    if (milliseconds > 500)
                    {
                        Database.accountsRequests.RemoveAt(i);
                        i--;
                    }
                }
            }

            lock (Database.paymentsRequestsLock)
            {
                for (int i = 0; i < Database.accountsRequests.Count; i++)
                {
                    TimeSpan time = DateTime.Now - Database.accountsRequests[i].TimeOfCreation;
                    double milliseconds = time.Milliseconds;
                    if (milliseconds > 500)
                    {
                        Database.accountsRequests.RemoveAt(i);
                        i--;
                    }
                }
            }
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
