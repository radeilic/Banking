using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Services;
using Common.Certifications;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Common.Auditing;
using System.ServiceModel.Description;

namespace BankingService
{
    class Program
    {
        private static BankingServiceIDSProxy proxy;
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            EndpointAddress address = new EndpointAddress(new Uri(ConfigurationManager.AppSettings["BankingServiceIDSAddress"]));
            proxy = new BankingServiceIDSProxy(binding, address);
            string srvCertCN = ConfigurationManager.AppSettings["serverCertificationCN"];

            NetTcpBinding adminsBinding = new NetTcpBinding();
            string adminsAddress = ConfigurationManager.AppSettings["adminServicesAddress"];
            ServiceHost adminsSvcHost = new ServiceHost(typeof(AdminServices));
            adminsSvcHost.AddServiceEndpoint(typeof(IAdminServices), adminsBinding, adminsAddress);

            adminsBinding.Security.Mode = SecurityMode.Transport;
            adminsBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            adminsSvcHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            adminsSvcHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
            adminsSvcHost.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            adminsSvcHost.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);


            ServiceSecurityAuditBehavior newAuditAdminService = new ServiceSecurityAuditBehavior();
            newAuditAdminService.AuditLogLocation = AuditLogLocation.Application;

            newAuditAdminService.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
            newAuditAdminService.SuppressAuditFailure = true;

            adminsSvcHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            adminsSvcHost.Description.Behaviors.Add(newAuditAdminService);

            try
            {
                adminsSvcHost.Open();
                Console.WriteLine("AdminServices is opened...");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while trying to open host for admins {e.Message}");
                Console.WriteLine($"[StackTrace] {e.StackTrace}");
                adminsSvcHost.Close();

                return;
            }
            


            NetTcpBinding usersBinding = new NetTcpBinding();
            string usersAddress = ConfigurationManager.AppSettings["userServicesAddress"];
            ServiceHost usersSvcHost = new ServiceHost(typeof(UserServices));
            usersSvcHost.AddServiceEndpoint(typeof(IUserServices), usersBinding, usersAddress);

            usersBinding.Security.Mode = SecurityMode.Transport;
            usersBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            usersSvcHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            usersSvcHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
            usersSvcHost.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            usersSvcHost.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);


            ServiceSecurityAuditBehavior newAuditUserService = new ServiceSecurityAuditBehavior();
            newAuditUserService.AuditLogLocation = AuditLogLocation.Application;

            newAuditUserService.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
            newAuditUserService.SuppressAuditFailure = true;

            usersSvcHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            usersSvcHost.Description.Behaviors.Add(newAuditUserService);

            try
            {
                usersSvcHost.Open();
                Console.WriteLine("UserServices is opened...");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while trying to open host for admins {e.Message}");
                Console.WriteLine($"[StackTrace] {e.StackTrace}");
                adminsSvcHost.Close();
                usersSvcHost.Close();

                return;
            }

            Thread OpenAccountSectorThread = new Thread(Program.OpenAccountSector);
            OpenAccountSectorThread.Start();
            Thread PaymentSectorThread = new Thread(Program.PaymentSector);
            PaymentSectorThread.Start();
            Thread LoansSectorThread = new Thread(Program.LoansSector);
            LoansSectorThread.Start();

            Console.WriteLine("Press any key to close server.");
            Console.ReadKey();

            adminsSvcHost.Close();
            usersSvcHost.Close();

            OpenAccountSectorThread.Abort();
            PaymentSectorThread.Abort();
            LoansSectorThread.Abort();
        }

        static void OpenAccountSector()
        {
            while (true)
            {
                if (Database.AccountRequests != null)
                {
                    lock (Database.AccountRequestsLock)
                    {
                        if (Database.AccountRequests.Count > 0)
                        {

                            Request request = Database.AccountRequests[Database.AccountRequests.Count - 1];
                            lock (Database.AccountsLock)
                            {
                                if (Database.Accounts.ContainsKey(request.Account.AccountName))
                                {
                                    Database.AccountRequests.Remove(request);
                                    request.State = RequestState.REJECTED;
                                }
                                else
                                {
                                    Random random = new Random();
                                    request.Account.PIN = random.Next(1000, 9999);
                                    Database.Accounts.Add(request.Account.AccountName, request.Account);
                                    Console.WriteLine("Account added.");
                                    Database.AccountRequests.Remove(request);
                                    request.State = RequestState.PROCCESSED;
                                }
                            }
                        }
                    }

                    if (Database.AccountRequests.Count == 0)
                    {
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        static void PaymentSector()
        {
            while (true)
            {
                if (Database.PaymentRequests != null)
                {
                    lock (Database.PaymentRequestsLock)
                    {
                        if (Database.PaymentRequests.Count > 0)
                        {
                            Request request = Database.PaymentRequests[Database.PaymentRequests.Count - 1];
                            IDSResult idsResult = proxy.Check(request);

                            if (!CheckIDSResult(idsResult, request))
                                continue;

                            if (request.IsOutgoing)
                            {
                                lock (Database.AccountsLock)
                                {
                                    request.Account.Amount += request.Amount;
                                    request.State = RequestState.PROCCESSED;
                                    Database.PaymentRequests.Remove(request);
                                }
                            }
                            else
                            {
                                lock (Database.AccountsLock)
                                {
                                    if (request.Account.Amount >= request.Amount)
                                    {
                                        request.Account.Amount -= request.Amount;
                                        request.State = RequestState.PROCCESSED;
                                        Database.PaymentRequests.Remove(request);
                                    }
                                    else
                                    {
                                        request.State = RequestState.REJECTED;
                                        Database.PaymentRequests.Remove(request);

                                    Audit.CustomLog.Source = "UserServices.Payment";
                                    Audit.UserOperationFailed(request.Account.Owner, "Payment", "Insufficient funds");
                                }
                            }
                        }
                    }
                    if (Database.PaymentRequests.Count == 0)
                    {
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        static void LoansSector()
        {
            while (true)
            {
                if (Database.LoanRequests != null)
                {
                    lock (Database.LoanRequestsLock)
                    {
                        if (Database.LoanRequests.Count > 0)
                        {
                            Request request = Database.LoanRequests[Database.LoanRequests.Count - 1];
                            IDSResult idsResult = proxy.Check(request);

                            if (!CheckIDSResult(idsResult, request))
                                continue;

                            lock (Database.AccountsLock)
                            {
                                if (request.Amount > 0)
                                {
                                    request.Account.Amount += request.Amount;
                                    Database.LoanRequests.Remove(request);
                                    request.State = RequestState.PROCCESSED;
                                }
                                else
                                {
                                    Database.LoanRequests.Remove(request);
                                    request.State = RequestState.REJECTED;

                                    Audit.CustomLog.Source = "UserServices.RaiseALoan";
                                    Audit.UserOperationFailed(request.Account.Owner, "RaiseALoan", "Invalid value to raise");
                                }
                            }

                        }
                    }
                    if (Database.LoanRequests.Count == 0)
                    {
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        static private bool CheckIDSResult(IDSResult idsResult, Request request)
        {
            bool retVal = false;

            switch (idsResult)
            {
                case IDSResult.BlockForDailyLimit:
                    request.Account.IsBlocked = true;
                    request.Account.BlockedUntil = DateTime.Now.AddDays(Int32.Parse(ConfigurationManager.AppSettings["daysLockForLimitViolation"]));

                    request.State = RequestState.REJECTED;
                    Database.PaymentRequests.Remove(request);

                    Audit.CustomLog.Source = " UserServices.Payment";
                    Audit.UserOperationFailed(request.Account.Owner, "Payment", "Daily limit reached");

                    retVal = false;
                    break;
                case IDSResult.BlockForOverload:
                    request.Account.IsBlocked = true;
                    request.Account.BlockedUntil = DateTime.Now.AddDays(Int32.Parse(ConfigurationManager.AppSettings["daysLockForOverload"]));

                    request.State = RequestState.REJECTED;
                    Database.PaymentRequests.Remove(request);

                    Audit.CustomLog.Source = " UserServices.Payment";
                    Audit.UserOperationFailed(request.Account.Owner, "Payment", "Server overload");

                    retVal = false;
                    break;
                case IDSResult.BlockForWrongPIN:
                    request.Account.IsBlocked = true;
                    request.Account.BlockedUntil = DateTime.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["minutesLockForWrongPin"]));

                    request.State = RequestState.REJECTED;
                    Database.PaymentRequests.Remove(request);

                    Audit.CustomLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed(request.Account.Owner, "Payment", "Wrong PIN");
                    retVal = false;
                    break;
                case IDSResult.FailedPayment:
                    request.State = RequestState.REJECTED;
                    Database.PaymentRequests.Remove(request);

                    Audit.CustomLog.Source = "UserServices.Payment";
                    Audit.UserOperationFailed(request.Account.Owner, "Payment", "Wrong PIN");

                    retVal = false;
                    break;
                case IDSResult.FailedLoan:
                    request.State = RequestState.REJECTED;
                    Database.LoanRequests.Remove(request);

                    Audit.CustomLog.Source = "UserServices.RaiseALoan";
                    Audit.UserOperationFailed(request.Account.Owner, "RaiseALoan", "Wrong PIN");

                    retVal = false;
                    break;
                case IDSResult.Exception:
                    request.State = RequestState.REJECTED;
                    Database.PaymentRequests.Remove(request);

                    retVal = false;
                    break;
                case IDSResult.OK:
                    retVal = true;
                    break;
            }

            return retVal;
        }
    }
}
