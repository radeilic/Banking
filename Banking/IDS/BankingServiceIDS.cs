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
                    Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Payment failed becaouse of request overload.");

                    account.IntervalBeginning = null;
                    return IDSResult.BlockForOverload;
                }

                if (request.Account.PIN != request.PIN)
                {
                    if (account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                       
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Payment failed. Account blocked for wrong PIN.");

                        account.LoginAttempts = 0;
                        return IDSResult.BlockForWrongPIN;
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Payment failed because of wrong PIN.");
                    account.LoginAttempts++;
                    return IDSResult.FailedPayment;
                }
                else
                {
                    account.LoginAttempts = 0;
                }

                if (!request.IsOutgoing)
                {
                    if (request.Account.Amount < request.Amount)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Payment failed because of insufficient funds.");
                        return IDSResult.FailedPayment;
                    }
                    else if ((account.DailyAmount + request.Amount) > Int32.Parse(ConfigurationManager.AppSettings["maxDailyIncomingAmount"]))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Payment failed. Account blocked because of daily limit.");

                        account.DailyAmount = 0;
                        return IDSResult.BlockForDailyLimit;
                    }

                    account.DailyAmount += request.Amount;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Payment done.");
                    return IDSResult.OK;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Payment done.");
                return IDSResult.OK;

            }
            else if(request.Type==RequestType.RaiseALoan)
            {
                if (CheckIfRequestsOverload(account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Raise a loan failed because of request overload.");

                    account.IntervalBeginning = null;
                    return IDSResult.BlockForOverload;
                }

                if (request.Account.PIN != request.PIN)
                {
                    if (account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Raise a loan failed. Account blocked for wrong PIN.");

                        account.LoginAttempts = 0;
                        return IDSResult.BlockForWrongPIN;
                    }

                    account.LoginAttempts++;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Raise a loan failed because of wrong PIN.");
                    return IDSResult.FailedLoan;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{DateTime.Now} - {account.AccountName} - Raise a loan succcessful!");
                    account.LoginAttempts = 0;
                    return IDSResult.OK;
                }
            }
            
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
                else if (account.RequestsCount + 1 >
                         Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadLimit"]))
                    retVal = true;
                else
                    ++account.RequestsCount;
            }

            return retVal;
        }
    }
}
