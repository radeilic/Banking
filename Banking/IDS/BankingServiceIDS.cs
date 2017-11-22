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
            if(request.Type==RequestType.Payment)
            {

                if( CheckIfRequestsOverload(request.Account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Payment failed.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return IDSResult.BlockForOverload;
                }

                if (request.Account.PIN != request.PIN)
                {
                    if (request.Account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                       
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Payment failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return IDSResult.BlockForWrongPIN;
                    }

                    Console.WriteLine("Payment failed.");
                    request.Account.LoginAttempts++;
                    return IDSResult.FailedPayment;
                }
                else
                {

                    request.Account.LoginAttempts = 0;
                }

                if (!request.IsOutgoing)
                {

                    if (request.Account.Amount >= request.Amount)
                    {
                        if ((request.Account.DailyAmount + request.Amount) > Int32.Parse(ConfigurationManager.AppSettings["maxDailyIncomingAmount"]))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Raise a loan failed. Account Blocked.");
                            Console.ForegroundColor = ConsoleColor.White;
                            return IDSResult.BlockForDailyLimit;
                        }

                        request.Account.Amount -= request.Amount;
                        request.Account.DailyAmount += request.Amount;

                        Console.WriteLine("Payment done!");
                        return IDSResult.OK;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Payment failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return IDSResult.FailedPayment;
                    }
                }
            }
            else if(request.Type==RequestType.RaiseALoan)
            {
                if (CheckIfRequestsOverload(request.Account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Raise a loan failed.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return IDSResult.BlockForOverload;
                }

                if (request.Account.PIN != request.PIN)
                {
                    if (request.Account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Raise a loan failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return IDSResult.BlockForWrongPIN;
                    }

                    request.Account.LoginAttempts++;
                    Console.WriteLine("Raise a loan failed!");
                    return IDSResult.FailedLoan;
                }
                else
                {
                    Console.WriteLine("Raise a loan succcessful!");
                    request.Account.LoginAttempts = 0;
                    return IDSResult.OK;
                }


            }
            
            Console.WriteLine("Payment outgoing done!");
            return IDSResult.OK;
        }

        public bool CheckIfRequestsOverload(Account account)
        {
            bool retVal = false;

            lock (account)
            {
                if (account.IntevalBeginning == null || account.IntevalBeginning > DateTime.Now.AddSeconds(Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadCheckInterval"])))
                {
                    Console.WriteLine("prvi "+account.IntevalBeginning);
                    Console.WriteLine("drugi "+ DateTime.Now.AddSeconds(Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadCheckInterval"])));
                    Database.Accounts[account.AccountName].IntevalBeginning = DateTime.Now;
                    account.RequestsCount = 1;
                }
                else if (account.RequestsCount + 1 >
                         Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadLimit"]))
                {
                    Console.WriteLine("else if");
                    retVal = true;
                }
                else
                {
                    Console.WriteLine("else");
                    ++account.RequestsCount;
                }
            }

            return retVal;
        }
    }
}
