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
        public bool OpenAccount(string accountName)
        {
            string owner = WindowsIdentity.GetCurrent().Name;
            Account account = new Account(owner, accountName);

            foreach (Account a in Database.accounts.Values)
            {
                if (account.AccountName == a.AccountName)
                {
                    Console.WriteLine("Account already in use!");
                    return false;
                }
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
                return true;
            } 
            else
            {
                return false;
            }
        }

        /// <inheritdoc />
        public bool Payment(bool isPayment, string accountName, int amount)
        {

            foreach (Account a in Database.accounts.Values)
            {
                if (accountName == a.AccountName)
                {
                    DateTime now = DateTime.Now;

                    //true is for + payment
                    Request request = new Request(now, a, amount, isPayment);

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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;

        }

        /// <inheritdoc />
        public bool RaiseALoan(string accountName, int amount)
        {
            foreach (Account a in Database.accounts.Values)
            {
                if (accountName == a.AccountName)
                {
                    DateTime now = DateTime.Now;

                    //true is for + payment
                    Request request = new Request(now, a, amount);

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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
