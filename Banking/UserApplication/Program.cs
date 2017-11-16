using Common.Certifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
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
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:25001/UserServices"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (UserProxy proxy = new UserProxy(binding, address))
            {
                proxy.OpenAccount("123");
                proxy.RaiseALoan("123", 123);
                proxy.Payment(true, "123", 123);
            }
            Console.WriteLine("Press any key to close UserApp.");
            Console.ReadKey(true);
        }
    }
}
