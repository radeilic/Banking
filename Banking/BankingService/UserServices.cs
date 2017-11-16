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
        public bool OpenAccount(string accountName)
        {
            string owner = WindowsIdentity.GetCurrent().Name;
            Account account = new Account(owner, accountName);

            foreach(Account a in Database.accounts)
            {
                if(account.AccountName==a.AccountName)
                {
                    Console.WriteLine("Account already in use!");
                    return false;
                }
            }

            DateTime now = DateTime.Now;

            Request request = new Request(now, account);

            lock (Database.accountsRequests)
            {
                Database.accountsRequests.Enqueue(request);
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

        public bool Payment(bool isPayment, string accountName, int amount)
        {
            if(isPayment)
            {

            }
            else
            {

            }

            return false;

        }

        public bool RaiseALoan(string accountName, int amount)
        {
            Console.WriteLine("RaiseALoan called.");
            return true;
        }
    }
}
