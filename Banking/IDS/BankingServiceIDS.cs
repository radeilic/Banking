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
        public bool Check(Request request, int pin)
        {
            if(request.Type==RequestType.Payment)
            {

                if( CheckIfRequestsOverload(request.Account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Payment failed.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return true;
                }

                if (request.Account.PIN != pin)
                {
                    if (request.Account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                       
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Payment failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return true;
                    }

                    request.Account.LoginAttempts++;
                   
                }

                if (!request.IsOutgoing)
                {
                    if ((request.Account.DailyAmount + request.Amount) > Int32.Parse(ConfigurationManager.AppSettings["maxDailyIncomingAmount"]))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Raise a loan failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return true;
                    }
                    
                }

                Console.WriteLine("Payment done!");
                return false;
            }
            else if(request.Type==RequestType.RaiseALoan)
            {
                if (CheckIfRequestsOverload(request.Account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Raise a loan failed.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return true;
                }

                if (request.Account.PIN != pin)
                {
                    if (request.Account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Raise a loan failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return true;
                    }

                    request.Account.LoginAttempts++;
                    Console.WriteLine("Raise a loan done!");
                    return false;
                }

               
            }
            
            Console.WriteLine("");
            return false;
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
                    retVal = true;
                }
                else
                    ++account.RequestsCount;
            }

            return retVal;
        }
    }
}
