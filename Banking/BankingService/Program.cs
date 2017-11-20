using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();
            string address1 = "net.tcp://localhost:25000/AdminServices";
            ServiceHost host1 = new ServiceHost(typeof(AdminServices));
            host1.AddServiceEndpoint(typeof(IAdminServices), binding, address1);

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            host1.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host1.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
            host1.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host1.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            //ServiceSecurityAuditBehavior newAuditAdminService = new ServiceSecurityAuditBehavior();
            //newAuditAdminService.AuditLogLocation = AuditLogLocation.Application;

            //newAuditAdminService.MessageAuthenticationAuditLevel = AuditLevel.SuccessOrFailure;
            //newAuditAdminService.SuppressAuditFailure = true;

            //host1.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            //host1.Description.Behaviors.Add(newAuditAdminService);

            try
            {
                host1.Open();
                Console.WriteLine("AdminServices is opened...");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while trying to open host for admins {e.Message}");
                Console.WriteLine($"[StackTrace] {e.StackTrace}");
                host1.Close();

                return;
            }
            


            NetTcpBinding binding2 = new NetTcpBinding();
            string address2 = "net.tcp://localhost:25001/UserServices";
            ServiceHost host2 = new ServiceHost(typeof(UserServices));
            host2.AddServiceEndpoint(typeof(IUserServices), binding2, address2);

            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            host2.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host2.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
            host2.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host2.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);


            ServiceSecurityAuditBehavior newAuditUserService = new ServiceSecurityAuditBehavior();
            newAuditUserService.AuditLogLocation = AuditLogLocation.Application;

            newAuditUserService.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
            newAuditUserService.SuppressAuditFailure = true;

            host2.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host2.Description.Behaviors.Add(newAuditUserService);

            try
            {
                host2.Open();
                Console.WriteLine("UserServices is opened...");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while trying to open host for admins {e.Message}");
                Console.WriteLine($"[StackTrace] {e.StackTrace}");
                host1.Close();
                host2.Close();

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

            host1.Close();
            host2.Close();
        }
        static void OpenAccountSector()
        {
            while (true)
            {
                if (Database.accountsRequests != null)
                {
                    if (Database.accountsRequests.Count > 0)
                    {
                        lock (Database.accountRequestsLock)
                        {
                            Request req = Database.accountsRequests[Database.accountsRequests.Count - 1];

                            lock (Database.accountsLock)
                            {
                                Database.accounts.Add(req.Account.AccountName, req.Account);
                                Console.WriteLine("Account added.");
                                Database.accountsRequests.Remove(req);
                                req.State = RequestState.PROCCESSED;
                            }
                        }
                    }
                    else
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
                if (Database.paymentRequests != null)
                {
                    if (Database.paymentRequests.Count > 0)
                    {
                        lock (Database.paymentsRequestsLock)
                        {
                            Request req = Database.paymentRequests[Database.paymentRequests.Count - 1];

                            if (req.IsPayment)
                            {
                                lock (Database.accountsLock)
                                {
                                    req.Account.Amount += req.Amount;
                                    req.State = RequestState.PROCCESSED;
                                    Database.paymentRequests.Remove(req);
                                }
                            }
                            else
                            {
                                lock (Database.accountsLock)
                                {
                                    if (req.Account.Amount >= req.Amount)
                                    {
                                        req.Account.Amount -= req.Amount;
                                        req.State = RequestState.PROCCESSED;
                                        Database.paymentRequests.Remove(req);
                                    }
                                    else
                                    {
                                        req.State = RequestState.REJECTED;
                                        Database.paymentRequests.Remove(req);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static void LoansSector()
        {
            while (true)
            {
                if (Database.loansRequests != null)
                {
                    if (Database.loansRequests.Count > 0)
                    {
                        lock (Database.loansRequestsLock)
                        {
                            Request req = Database.loansRequests[Database.loansRequests.Count - 1];

                            lock (Database.accountsLock)
                            {
                                if (req.Amount > 0)
                                {
                                    req.Account.Amount += req.Amount;
                                    Database.loansRequests.Remove(req);
                                    req.State = RequestState.PROCCESSED;
                                }
                                else
                                {
                                    Database.loansRequests.Remove(req);
                                    req.State = RequestState.REJECTED;
                                }
                            }
                        }
                    }
                }
            }
        }
        

    }
}
