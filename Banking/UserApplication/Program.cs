using Common.Certifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string srvCertCN = "BankingService";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:25001/UserServices"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (UserProxy proxy = new UserProxy(binding, address))
            {
                string odabir;

                do
                {
                    Console.WriteLine("================Menu==============");
                    Console.WriteLine("**********************************");
                    Console.WriteLine();
                    Console.WriteLine("1. Open account");
                    Console.WriteLine("2. Raise a loan");
                    Console.WriteLine("3. Payment");
                    Console.WriteLine();
                    Console.WriteLine("==================================");
                    odabir = Console.ReadLine();

                    string accountName = "";
                    string pin = "";
                    string amount = "";

                    switch (odabir)
                    {
                        case "1":
                            Console.WriteLine("Enter account name:");
                            accountName = Console.ReadLine();
                            int res = proxy.OpenAccount(accountName);
                            if (res < 0)
                            {
                                Console.WriteLine("Failed to create account.");
                            }
                            else
                            {
                                Console.WriteLine("Your PIN is: " + res);
                            }
                            break;
                        case "2":
                            Console.WriteLine("Enter account name:");
                            accountName = Console.ReadLine();
                            Console.WriteLine("Enter PIN:");
                            pin = Console.ReadLine();
                            Console.WriteLine("Enter amount:");
                            amount = Console.ReadLine();

                            bool res2 = proxy.RaiseALoan(accountName, Int32.Parse(amount), Int32.Parse(pin));
                            if (res2)
                            {
                                Console.WriteLine("Loan raised.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to raise a loan.");
                            }
                            break;
                    }

                } while (odabir != "1" && odabir != "2" && odabir != "3");
            }
            Console.WriteLine("Press any key to close UserApp.");
            Console.ReadKey(true);
        }
    }
}
