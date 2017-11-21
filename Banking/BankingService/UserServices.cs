using Common.Auditing;
using Common.Services;
using System;
using System.Collections.Generic;
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

            if(Database.accounts.ContainsKey(accountName))
            {
                Console.WriteLine("Account already in use!");

                Audit.customLog.Source = "UserServices.OpenAccount";
                Audit.UserOperationFailed("Banking User", "OpenAccount", "Account already in use!");
                return -1;
            }

            DateTime now = DateTime.Now;
            Request request = new Request(now, account, 0);

            lock (Database.accountRequestsLock)
            {
                Database.accountsRequests.Insert(0, request);
            }

            while (request.State == RequestState.WAIT) 
            {
                Thread.Sleep(1000);
            }
            
            if(request.State==RequestState.PROCCESSED)
            {
                Audit.customLog.Source = "UserServices.OpenAccount";
                Audit.UserOperationSuccess("Banking User", "OpenAccount");
                return request.Account.PIN;
            } 
            else
            {
                Audit.customLog.Source = "UserServices.OpenAccount";
                Audit.UserOperationFailed("Banking User", "OpenAccount", "Request rejected");
                return -1;
            }
        }
        

        public bool Payment(bool isPayment, string accountName, int amount, int pin)
        {
            if(Database.accounts.ContainsKey(accountName))
            {
                Account account;

                lock (Database.accountsLock)
                {
                    account = Database.accounts[accountName];
                }

                if (CheckIfAccountIsBlocked(account))
                {
                    Audit.customLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed("Banking User", "Payment", "Account is blocked!");
                    return false;
                }
                    

                if (CheckIfRequestsOverload(account))
                {
                    Audit.customLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed("Banking User", "Payment", "Account is blocked / Server overload");
                    return false;
                }

                if (account.PIN != pin)
                {
                    Audit.customLog.Source = "UserServices.Payment";
                    Audit.Admin_User_Authentication_Authorization_Failed();

                    if (account.LoginAttempts == 2)
                    {
                        account.IsBlocked = true;
                        account.BlockedUntil = DateTime.Now.AddMinutes(1);

                        Audit.customLog.Source = "UserServices.Payment";
                        Audit.UserOperationFailed("Banking User", "Payment", "Account is blocked");
                        return false;
                    }
                    else
                    {
                        account.LoginAttempts++;
                        return false;
                    }
                }

                account.LoginAttempts = 0;
                DateTime now = DateTime.Now;

                //true is for + payment
                Request request = new Request(now, account, amount, isPayment);

                lock (Database.paymentsRequestsLock)
                {
                    Database.paymentRequests.Insert(0, request);
                }

                while (request.State == RequestState.WAIT)
                {
                    Thread.Sleep(1000);
                }

                if (request.State == RequestState.PROCCESSED)
                {
                    Audit.customLog.Source = "UserServices.Payment";
                    Audit.UserOperationSuccess("Banking User", "Payment");
                    return true;
                }
                else
                {
                    Audit.customLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed("Banking User", "Payment", "Request is rejected");
                    return false;
                }
                
            }

            Audit.customLog.Source = "UserServices.Payment";
            Audit.UserOperationFailed("Banking User", "Payment", "No account information in database");
            return false;

        }
        

        public bool RaiseALoan(string accountName, int amount, int pin)
        {
            if(Database.accounts.ContainsKey(accountName))
            {
                Account account;

                lock(Database.accountsLock)
                {
                    account = Database.accounts[accountName];
                }

                if (CheckIfAccountIsBlocked(account))
                {
                    Audit.customLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationFailed("Banking User", "RaiseALoan", "Account is blocked!");
                    return false;
                }


                if (CheckIfRequestsOverload(account))
                {
                    Audit.customLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationFailed("Banking User", "RaiseALoan", "Account is blocked / Server overload");
                    return false;
                }

                if (account.PIN != pin)
                {
                    Audit.customLog.Source = "UserServices.RaiseALoan";
                    Audit.Admin_User_Authentication_Authorization_Failed();

                    if(account.LoginAttempts==2)
                    {
                        account.IsBlocked = true;
                        account.BlockedUntil = DateTime.Now.AddMinutes(1);

                        Audit.customLog.Source = "UserServices.RaiseALoan";
                        Audit.UserOperationFailed("Banking User", "Payment", "Account is blocked");
                        return false;
                    }
                    else
                    {
                        account.LoginAttempts++;
                        return false;
                    }
                }

                account.LoginAttempts = 0;
                DateTime now = DateTime.Now;

                //true is for + payment
                Request request = new Request(now, account, amount);

                lock (Database.loansRequestsLock)
                {
                    Database.loansRequests.Insert(0, request);
                }

                while (request.State == RequestState.WAIT)
                {
                    Thread.Sleep(1000);
                }

                if (request.State == RequestState.PROCCESSED)
                {

                    Audit.customLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationSuccess("Banking User", "RaiseALoan");
                    return true;
                }
                else
                {
                    Audit.customLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationFailed("Banking User", "Payment", "Request is rejected");
                    return false;
                }
                
            }

            Audit.customLog.Source = "UserServices.RaiseALoan";
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
                if (account.IntevalBeginning == null || account.IntevalBeginning > DateTime.Now.AddSeconds(30))
                {
                    account.IntevalBeginning = DateTime.Now;
                    account.RequestsCount = 1;
                }
                else if (account.RequestsCount + 1 > 10)
                {
                    account.IsBlocked = true;
                    account.BlockedUntil = DateTime.Now.AddDays(1);
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
