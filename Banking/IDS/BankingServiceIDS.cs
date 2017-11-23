using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Configuration;

namespace IDS
{
    public class BankingServiceIDS : IBankingService
    {
        public IDSResult Check(Request request)
        {
            AccountIDS account;

            if(!DatabaseIDS.Accounts.ContainsKey(request.Account.AccountName))
            {
                account = new AccountIDS(request.Account.AccountName);
                DatabaseIDS.Accounts[account.AccountName]=account;
            }
            else
            {
                account = DatabaseIDS.Accounts[request.Account.AccountName];
            }

            if (DateTime.Now.Date > account.CurrentDay)
            {
                account.CurrentDay = DateTime.Now.Date;
                account.DailyAmount = 0;
            }


            if (request.Type==RequestType.Payment)
            {

                if( CheckIfRequestsOverload(account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Payment failed.");
                    Console.ForegroundColor = ConsoleColor.White;

                    account.IntervalBeginning = null;
                    return IDSResult.BlockForOverload;
                }

                if (request.Account.PIN != request.PIN)
                {
                    if (account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                       
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Payment failed.");
                        Console.ForegroundColor = ConsoleColor.White;

                        account.LoginAttempts = 0;
                        return IDSResult.BlockForWrongPIN;
                    }

                    account.LoginAttempts++;
                    return IDSResult.FailedPayment;
                }
                else
                {
                    account.LoginAttempts = 0;
                }

                if (!request.IsOutgoing)
                {
                    if ((account.DailyAmount + request.Amount) > Int32.Parse(ConfigurationManager.AppSettings["maxDailyIncomingAmount"]))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Raise a loan failed.");
                        Console.ForegroundColor = ConsoleColor.White;

                        account.DailyAmount = 0;
                        return IDSResult.BlockForDailyLimit;
                    }

                    account.DailyAmount += request.Amount;
                }

                Console.WriteLine("Payment done!");
                return IDSResult.OK;
            }
            else if(request.Type==RequestType.RaiseALoan)
            {
                if (CheckIfRequestsOverload(account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Raise a loan failed.");
                    Console.ForegroundColor = ConsoleColor.White;

                    account.IntervalBeginning = null;
                    return IDSResult.BlockForOverload;
                }

                if (request.Account.PIN != request.PIN)
                {
                    if (account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Raise a loan failed.");
                        Console.ForegroundColor = ConsoleColor.White;

                        account.LoginAttempts = 0;
                        return IDSResult.BlockForWrongPIN;
                    }

                    account.LoginAttempts++;
                    Console.WriteLine("Raise a loan failed!");
                    return IDSResult.FailedLoan;
                }
                else
                {
                    account.LoginAttempts = 0;
                    return IDSResult.OK;
                }


            }
            
            Console.WriteLine("Ne treba ovde da dodje!");
            return IDSResult.OK;
        }

        public bool CheckIfRequestsOverload(AccountIDS account)
        {
            bool retVal = false;

            lock (account)
            {
                if (account.IntervalBeginning == null || account.IntervalBeginning > DateTime.Now.AddSeconds(Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadCheckInterval"])))
                {
                    account.IntervalBeginning = DateTime.Now;
                    account.RequestsCount = 1;
                }
                else if (account.RequestsCount + 1 > Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadLimit"]))
                {
                    retVal = true;
                }
                else
                    ++account.RequestsCount;
            }

            return retVal;
        }
    }
}
