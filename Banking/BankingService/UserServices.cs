using Common.Auditing;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class UserServices : IUserServices
    {
        /// <inheritdoc />
        public int OpenAccount(string accountName)
        {
            string owner = WindowsIdentity.GetCurrent().Name;
            Account account = new Account(owner, accountName);

            if(Database.Accounts.ContainsKey(accountName))
            {
                Console.WriteLine("Account already in use!");

                Audit.CustomLog.Source = "UserServices.OpenAccount";
                Audit.UserOperationFailed("Banking User", "OpenAccount", "Account already in use!");
                return -1;
            }

            DateTime now = DateTime.Now;
            Request request = new Request(now, account, 0);

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
                Audit.UserOperationSuccess("Banking User", "OpenAccount");
                return request.Account.PIN;
            } 
            else
            {
                Audit.CustomLog.Source = "UserServices.OpenAccount";
                Audit.UserOperationFailed("Banking User", "OpenAccount", "Request rejected");
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
                    Audit.UserOperationFailed("Banking User", "Payment", "Account is blocked!");
                    return false;
                }

                if (CheckIfRequestsOverload(account))
                {
                    Audit.CustomLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed("Banking User", "Payment", "Server overload");
                    return false;
                }

                if (account.PIN != pin)
                {
                    Audit.CustomLog.Source = "UserServices.Payment";
                    Audit.AdminUserAuthenticationAuthorizationFailed();

                    if (account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"])-1)
                    {
                        account.IsBlocked = true;
                        account.BlockedUntil = DateTime.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["minutesLockForWrongPin"]));

                        Audit.CustomLog.Source = "UserServices.Payment";
                        Audit.UserOperationFailed("Banking User", "Payment", "Account is blocked");
                        return false;
                    }

                    account.LoginAttempts++;

                    Audit.CustomLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed("Banking User", "Payment", "Wrong PIN");
                    return false;
                }

                account.LoginAttempts = 0;
                DateTime now = DateTime.Now;

                //true is for + payment
                Request request = new Request(now, account, amount, isOutgoing);

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
                    Audit.UserOperationSuccess("Banking User", "Payment");
                    return true;
                }
                else
                {
                    Audit.CustomLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed("Banking User", "Payment", "Request is rejected");
                    return false;
                }
                
            }

            Audit.CustomLog.Source = "UserServices.Payment";
            Audit.UserOperationFailed("Banking User", "Payment", "No account information in database");
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
                    Audit.UserOperationFailed("Banking User", "RaiseALoan", "Account is blocked!");
                    return false;
                }

                if (CheckIfRequestsOverload(account))
                {
                    Audit.CustomLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationFailed("Banking User", "RaiseALoan", "Server overload");
                    return false;
                }

                if (account.PIN != pin)
                {
                    Audit.CustomLog.Source = "UserServices.RaiseALoan";
                    Audit.AdminUserAuthenticationAuthorizationFailed();

                    if(account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"])-1)
                    {
                        account.IsBlocked = true;
                        account.BlockedUntil = DateTime.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["minutesLockForWrongPin"]));

                        Audit.CustomLog.Source = "UserServices.RaiseALoan";
                        Audit.UserOperationFailed("Banking User", "Payment", "Account is blocked");
                        return false;
                    }

                    account.LoginAttempts++;

                    Audit.CustomLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationFailed("Banking User", "Payment", "Wrong PIN");
                    return false;
                }

                account.LoginAttempts = 0;
                DateTime now = DateTime.Now;

                //true is for + payment
                Request request = new Request(now, account, amount);

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
                    Audit.UserOperationSuccess("Banking User", "RaiseALoan");
                    return true;
                }
                else
                {
                    Audit.CustomLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationFailed("Banking User", "Payment", "Request is rejected");
                    return false;
                }
                
            }

            Audit.CustomLog.Source = "UserServices.RaiseALoan";
            Audit.UserOperationFailed("Banking User", "RaiseALoan", "No account information in database");
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

        public bool CheckIfRequestsOverload(Account account)
        {
            bool retVal = false;

            lock (account)
            {
                if (account.IntevalBeginning == null || account.IntevalBeginning > DateTime.Now.AddSeconds(Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadCheckInterval"])))
                {
                    account.IntevalBeginning = DateTime.Now;
                    account.RequestsCount = 1;
                }
                else if (account.RequestsCount + 1 > Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadLimit"]))
                {
                    account.IsBlocked = true;
                    account.BlockedUntil = DateTime.Now.AddDays(Int32.Parse(ConfigurationManager.AppSettings["daysLockForOverload"]));
                    account.IntevalBeginning = null;
                    retVal = true;
                }
                else
                    ++account.RequestsCount;
            }

            return retVal;
        }
    }
}
