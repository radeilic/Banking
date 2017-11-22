using Common;
using Common.Auditing;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BankingService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AdminServices : IAdminServices
    {
        public void CheckRequest()
        {
            Audit.CustomLog.Source = "AdminServices.CheckRequests";
            Audit.AdminUserAuthenticationAuthorizationSuccess();

            int waitingLimit = Int32.Parse(ConfigurationManager.AppSettings["waitingLimitInMiliseconds"]);

            lock (Database.AccountRequestsLock)
            {
                for (int i = 0; i < Database.AccountRequests.Count; ++i)
                {
                    TimeSpan time = DateTime.Now - Database.AccountRequests[i].TimeOfCreation;
                    double millisecondsWaiting = time.Milliseconds;

                    if (millisecondsWaiting > waitingLimit)
                    {
                        Database.AccountRequests[i].State = RequestState.REJECTED;
                        Database.AccountRequests.RemoveAt(i);
                        --i;

                        Audit.CustomLog.Source = "AdminServices.CheckRequest";
                        Audit.AdminOperationSuccess("CheckRequest");
                    }
                }
            }

            lock (Database.LoanRequestsLock)
            {
                for (int i = 0; i < Database.AccountRequests.Count; ++i)
                {
                    TimeSpan time = DateTime.Now - Database.AccountRequests[i].TimeOfCreation;
                    double millisecondsWaiting = time.Milliseconds;

                    if (millisecondsWaiting > waitingLimit)
                    {
                        Database.LoanRequests[i].State = RequestState.REJECTED;
                        Database.LoanRequests.RemoveAt(i);
                        --i;

                        Audit.CustomLog.Source = "AdminServices.CheckRequest";
                        Audit.AdminOperationSuccess("CheckRequest");
                    }
                }
            }

            lock (Database.PaymentRequestsLock)
            {
                for (int i = 0; i < Database.AccountRequests.Count; ++i)
                {
                    TimeSpan time = DateTime.Now - Database.AccountRequests[i].TimeOfCreation;
                    double millisecondsWaiting = time.Milliseconds;

                    if (millisecondsWaiting > waitingLimit)
                    {
                        Database.PaymentRequests[i].State = RequestState.REJECTED;
                        Database.PaymentRequests.RemoveAt(i);
                        --i;

                        Audit.CustomLog.Source = "AdminServices.CheckRequest";
                        Audit.AdminOperationSuccess("CheckRequest");
                    }
                }
            }

            Audit.CustomLog.Source = "AdminServices.CheckRequests";
            Audit.AdminOperationSuccess("CheckRequests");
        }

        public void Init()
        {
            if (Database.Accounts == null)
            {
                Audit.CustomLog.Source = "AdminServices.Init";
                Audit.AdminUserAuthenticationAuthorizationSuccess();

                Database.Accounts = new Dictionary<string, Account>();
                Database.AccountRequests = new List<Request>();
                Database.LoanRequests = new List<Request>();
                Database.PaymentRequests = new List<Request>();

                Audit.CustomLog.Source = "AdminServices.Init";
                Audit.AdminOperationSuccess("Init");
            }
        }
    }
}
