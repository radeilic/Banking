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
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://10.1.212.111:25001/UserServices"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (UserProxy proxy = new UserProxy(binding, address))
            {
                string odabir;

                do
                {
                    Console.WriteLine();
                    Console.WriteLine("================Menu==============");
                    Console.WriteLine("**********************************");
                    Console.WriteLine();
                    Console.WriteLine("1. Open account");
                    Console.WriteLine("2. Raise a loan");
                    Console.WriteLine("3. Payment");
                    Console.WriteLine("4. Test overload");
                    Console.WriteLine("5. Exit");
                    Console.WriteLine();
                    Console.WriteLine("==================================");
                    Console.WriteLine();
                    odabir = Console.ReadLine();

                    string accountName = "";
                    string pin = "";
                    string amount = "";
                    bool res2;

                    switch (odabir)
                    {
                        case "1":
                            Console.Write("Enter account name: ");
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
                            Console.Write("Enter account name: ");
                            accountName = Console.ReadLine();
                            Console.Write("Enter PIN: ");
                            pin = Console.ReadLine();
                            Console.Write("Enter amount: ");
                            amount = Console.ReadLine();

                            res2 = proxy.RaiseALoan(accountName, Int32.Parse(amount), Int32.Parse(pin));
                            if (res2)
                            {
                                Console.WriteLine("Loan raised.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to raise a loan.");
                            }
                            break;
                        case "3":
                            Console.Write("Enter account name: ");
                            accountName = Console.ReadLine();
                            Console.Write("Enter PIN: ");
                            pin = Console.ReadLine();
                            Console.WriteLine("1 - Pay the money");
                            Console.WriteLine("2 - Raise the money");
                            string res3 = Console.ReadLine();
                            bool choise = res3 == "1" ? true : false;
                            Console.Write("Enter amount: ");
                            amount = Console.ReadLine();

                            res2 = proxy.Payment(choise, accountName, Int32.Parse(amount),Int32.Parse(pin));
                            if (res2)
                            {
                                Console.WriteLine("Payment done.");
                            }
                            else
                            {
                                Console.WriteLine("Payment failed.");
                            }
                            break;
                        case "4":
                            int result = proxy.OpenAccount("Test Account");

                            for(int i=0; i < 11; ++i)
                            {
                                if (proxy.Payment(true, "Test Account", 100, result))
                                    Console.WriteLine($"Attempt {i + 1} succeeded");
                                else
                                    Console.WriteLine($"Attempt {i + 1} failed");

                            }

                            break;
                    }

                } while (odabir!="5");
            }
        }
    }
}
