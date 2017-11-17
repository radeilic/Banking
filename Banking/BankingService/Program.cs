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

            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
            host1.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host1.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
            host1.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host1.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

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
            binding2.Security.Mode = SecurityMode.Message;
            binding2.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
            string address2 = "net.tcp://localhost:25001/UserServices";
            ServiceHost host2 = new ServiceHost(typeof(UserServices));
            host2.AddServiceEndpoint(typeof(IUserServices), binding2, address2);

            binding2.Security.Mode = SecurityMode.Message;
            binding2.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
            host2.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host2.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
            host2.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host2.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

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




            Console.WriteLine("Press any key to close server.");
            Console.ReadKey();

            host1.Close();
            host2.Close();
        }



    }
}
