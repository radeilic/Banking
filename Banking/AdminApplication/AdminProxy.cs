using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common.Certifications;
using System.ComponentModel;
using System.Configuration;

namespace AdminApplication
{
    class AdminProxy : ChannelFactory<IAdminServices>, IAdminServices, IDisposable
    {
        IAdminServices factory;
        public AdminProxy(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
		    string cltCertCN = ConfigurationManager.AppSettings["adminClientCertificationCN"];

		    this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
		    this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
		    this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
		    this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

		    factory = this.CreateChannel();
        }

        public void CheckRequest()
        {
            try
            {
                factory.CheckRequest();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in CheckRequest: {e.Message}");
            }
        }

        public void Init()
        {
            try
            {
                factory.Init();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in Init: {e.Message}");
            }
        }
    }
}
