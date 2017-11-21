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

namespace AdminApplication
{
    class AdminProxy : ChannelFactory<IAdminServices>, IAdminServices, IDisposable
    {
        IAdminServices factory;
        public AdminProxy(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
		    string cltCertCN = "Admin";

		    this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
		    this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
		    this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
		    this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

		    factory = this.CreateChannel();
        }

        public bool CheckRequest()
        {
            try
            {
                return factory.CheckRequest();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in CheckRequest: {0}", e.Message);
                return false;
            }
        }

        public bool Init()
        {
            try
            {
                return factory.Init();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Init: {0}", e.Message);
                return false;
            }
        }
    }
}
