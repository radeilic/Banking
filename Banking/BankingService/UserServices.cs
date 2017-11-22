using Common.Auditing;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class UserServices : IUserServices
    {
        /// <inheritdoc />
        public int OpenAccount(string accountName)
        {
            string owner = Thread.CurrentPrincipal.Identity.Name;
            Account account = new Account(owner, accountName);

            if(Database.Accounts.ContainsKey(accountName))
            {
                Console.WriteLine("Account already in use!");

                Audit.CustomLog.Source = "UserServices.OpenAccount";
                Audit.UserOperationFailed(Thread.CurrentPrincipal.Identity.Name, "OpenAccount", "Account already in use!");
                return -1;
            }

            DateTime now = DateTime.Now;
            Request request = new Request(RequestType.OpenAccount, now, account, 0);

            lock (Database.AccountRequestsLock)
            {
                Database.AccountRequests.Insert(0, request);
            }

            while (request.State == RequestState.WAIT) 
            {
                Thread.Sleep(1000);
            }
            
            if(request.State==RequestState.PROCCESSED)
            {
                Audit.CustomLog.Source = "UserServices.OpenAccount";
                Audit.UserOperationSuccess(Thread.CurrentPrincipal.Identity.Name, "OpenAccount");
                return request.Account.PIN;
            } 
            else
            {
                return -1;
            }
        }
        
        public bool Payment(bool isOutgoing, string accountName, int amount, int pin)
        {
            if(Database.Accounts.ContainsKey(accountName))
            {
                Account account;

                lock (Database.AccountsLock)
                {
                    account = Database.Accounts[accountName];
                }

                if (CheckIfAccountIsBlocked(account))
                {
                    Audit.CustomLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed(account.Owner, "Payment", "Account is blocked!");
                    return false;
                }

                
                DateTime now = DateTime.Now;

                //true is for + payment
                Request request = new Request(now, account, pin, amount, isOutgoing);

                lock (Database.PaymentRequestsLock)
                {
                    Database.PaymentRequests.Insert(0, request);
                }

                while (request.State == RequestState.WAIT)
                {
                    Thread.Sleep(1000);
                }

                if (request.State == RequestState.PROCCESSED)
                {
                    Audit.CustomLog.Source = "UserServices.Payment";
                    Audit.UserOperationSuccess(account.Owner, "Payment");
                    return true;
                }
                else
                {
                    return false;
                }
                
            }

            Audit.CustomLog.Source = "UserServices.Payment";
            Audit.UserOperationFailed(Thread.CurrentPrincipal.Identity.Name, "Payment", "No account information in database");
            return false;

        }
        
        public bool RaiseALoan(string accountName, int amount, int pin)
        {
            if(Database.Accounts.ContainsKey(accountName))
            {
                Account account;

                lock(Database.AccountsLock)
                {
                    account = Database.Accounts[accountName];
                }

                if (CheckIfAccountIsBlocked(account))
                {
                    Audit.CustomLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationFailed(account.Owner, "RaiseALoan", "Account is blocked!");
                    return false;
                }

                

                DateTime now = DateTime.Now;

                //true is for + payment
                Request request = new Request(RequestType.RaiseALoan, now, account, pin, amount);

                lock (Database.LoanRequestsLock)
                {
                    Database.LoanRequests.Insert(0, request);
                }

                while (request.State == RequestState.WAIT)
                {
                    Thread.Sleep(1000);
                }

                if (request.State == RequestState.PROCCESSED)
                {

                    Audit.CustomLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationSuccess(account.Owner, "RaiseALoan");
                    return true;
                }
                else
                {
                    return false;
                }
                
            }

            Audit.CustomLog.Source = "UserServices.RaiseALoan";
            Audit.UserOperationFailed(Thread.CurrentPrincipal.Identity.Name, "RaiseALoan", "No account information in database");
            return false;
        }

        public bool CheckIfAccountIsBlocked(Account account)
        {
            lock (account)
            {
                if (account.IsBlocked)
                    if (DateTime.Now > account.BlockedUntil)
                        account.IsBlocked = false;

                return account.IsBlocked;
            }
        }

        
    }
}
