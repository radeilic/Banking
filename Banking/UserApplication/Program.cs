using Common.Certifications;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            string srvCertCN = ConfigurationManager.AppSettings["serverCertificationCN"];

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri(ConfigurationManager.AppSettings["userServicesAddress"]),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (UserProxy proxy = new UserProxy(binding, address))
            {
                string option;

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
                    option = Console.ReadLine();

                    string accountName = "";
                    string pin = "";
                    string amount = "";
                    bool result;
                    int newPIN;

                    switch (option)
                    {
                        case "1":
                            Console.Write("Enter account name: ");
                            accountName = Console.ReadLine();
                            newPIN = proxy.OpenAccount(accountName);

                            if (newPIN < 0)
                                Console.WriteLine("Failed to create account.");
                            else
                                Console.WriteLine($"Your PIN is: {newPIN}");

                            break;
                        case "2":
                            Console.Write("Enter account name: ");
                            accountName = Console.ReadLine();
                            Console.Write("Enter PIN: ");
                            pin = Console.ReadLine();
                            Console.Write("Enter amount: ");
                            amount = Console.ReadLine();

                            try
                            {
                                result = proxy.RaiseALoan(accountName, Int32.Parse(amount), Int32.Parse(pin));
                            }
                            catch
                            {
                                continue;
                            }

                            if (result)
                                Console.WriteLine("Loan raised.");
                            else
                                Console.WriteLine("Failed to raise a loan.");

                            break;
                        case "3":
                            Console.Write("Enter account name: ");
                            accountName = Console.ReadLine();
                            Console.Write("Enter PIN: ");
                            pin = Console.ReadLine();
                            Console.WriteLine("1 - Pay the money");
                            Console.WriteLine("2 - Raise the money");
                            bool choise = Console.ReadLine() == "1";
                            Console.Write("Enter amount: ");
                            amount = Console.ReadLine();

                            try
                            {
                                result = proxy.Payment(choise, accountName, Int32.Parse(amount), Int32.Parse(pin));
                            }
                            catch
                            {
                                continue;
                            }

                            if (result)
                                Console.WriteLine("Payment done.");
                            else
                                Console.WriteLine("Payment failed.");

                            break;
                        case "4":
                            newPIN = proxy.OpenAccount("Test Account");

                            for(int i=0; i < 11; ++i)
                            {
                                if (proxy.Payment(true, "Test Account", 100, newPIN))
                                    Console.WriteLine($"Attempt {i + 1} succeeded");
                                else
                                    Console.WriteLine($"Attempt {i + 1} failed");
                            }

                            break;
                    }

                } while (option!="5");
            }
        }
    }
}
