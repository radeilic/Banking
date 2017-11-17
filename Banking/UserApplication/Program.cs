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

                bool res=proxy.OpenAccount("123");
                if (res)
                {
                    Console.WriteLine("Account Opened");
                }
                res=proxy.RaiseALoan("123", 123);
                if (res)
                {
                    Console.WriteLine("Loan Raised.");
                }
                res = proxy.Payment(true, "123", 123);
                if (res)
                {
                    Console.WriteLine("Payment successful.");
                }
            }
            Console.WriteLine("Press any key to close UserApp.");
            Console.ReadKey(true);
        }
    }
}
