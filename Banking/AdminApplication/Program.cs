using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Certifications;
using Common.Auditing;

namespace AdminApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            if (!Thread.CurrentPrincipal.IsInRole(Formatter.FormatName(ConfigurationManager.AppSettings["adminGroupName"])))
            {
                Console.WriteLine("You don't have permission to use this component.");

                Audit.CustomLog.Source = "AdminService";
                Audit.AdminUserAuthenticationAuthorizationFailed();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);

                return;
            }


            string srvCertCN = ConfigurationManager.AppSettings["serverCertificationCN"];

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri(ConfigurationManager.AppSettings["adminServicesAddress"]),
                new X509CertificateEndpointIdentity(srvCert));

            Console.WriteLine("Press any key to close AdminApp.");

            using (AdminProxy proxy = new AdminProxy(binding, address))
            {
                proxy.Init();
                while(true)
                {
                    proxy.CheckRequest();

                    if (Console.KeyAvailable)
                        break;

                    Thread.Sleep(Int32.Parse(ConfigurationManager.AppSettings["requestsCheckIntervalInMiliseconds"]));
                }
            }
        }
    }
}
