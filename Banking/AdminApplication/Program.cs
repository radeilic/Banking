using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Certifications;

namespace AdminApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            if (!Thread.CurrentPrincipal.IsInRole(Formatter.FormatName("BankingSystemAdmin")))
            {
                Console.WriteLine("You don't have permission to use this component.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);

                return;
            }


            string srvCertCN = "BankingService";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:25000/AdminServices"),
                new X509CertificateEndpointIdentity(srvCert));

            using (AdminProxy proxy = new AdminProxy(binding, address))
            {
                proxy.Init();
                proxy.CheckRequest();
            }
            Console.WriteLine("Press any key to close AdminApp.");
            Console.ReadKey(true);
        }
    }
}
