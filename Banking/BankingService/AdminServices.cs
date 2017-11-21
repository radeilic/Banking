using Common;
using Common.Auditing;
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
            Audit.customLog.Source = "AdminServices.Init";
            Audit.Admin_User_Authentication_Authorization_Success();

            lock (Database.accountRequestsLock)
            {
                for (int i = 0;i < Database.accountsRequests.Count; i++)
                {
                    TimeSpan time = DateTime.Now - Database.accountsRequests[i].TimeOfCreation;
                    double milliseconds = time.Milliseconds;
                    if (milliseconds > 500)
                    {
                        Database.loansRequests[i].State = RequestState.REJECTED;
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
                        Database.loansRequests[i].State = RequestState.REJECTED;
                        Database.loansRequests.RemoveAt(i);
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
                        Database.paymentRequests[i].State = RequestState.REJECTED;
                        Database.paymentRequests.RemoveAt(i);
                        i--;
                    }
                }
            }

            Audit.customLog.Source = "AdminServices.CheckRequest";
            Audit.AdminOperationSuccess("CheckRequest");
            return true;
        }

        public bool Init()
        {
            Audit.customLog.Source = "AdminServices.Init";
            Audit.Admin_User_Authentication_Authorization_Success();

            Database.accounts = new Dictionary<string, Account>();
            Database.accountsRequests = new List<Request>();
            Database.loansRequests = new List<Request>();
            Database.paymentRequests = new List<Request>();

            Audit.customLog.Source = "AdminServices.Init";
            Audit.AdminOperationSuccess("Init");

            return true;
        }
    }
}
