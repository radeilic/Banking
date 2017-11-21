using Common.Certifications;
using Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication
{
    class UserProxy : ChannelFactory<IUserServices>, IUserServices, IDisposable
    {
        IUserServices factory;

        public UserProxy(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
            string cltCertCN = "User";

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }

        public int OpenAccount(string accountName)
        {
            try
            {
                return factory.OpenAccount(accountName);
            }
            catch(Win32Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in OpenAccount: {0}",e.Message);
                return -1;
            }
            
        }

        public bool Payment(bool isPayment,string accountName, int amount, int pin)
        {
            try
            {
                return factory.Payment(isPayment, accountName, amount, pin);
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Payment: {0}", e.Message);
                return false;
            }
        }

        public bool RaiseALoan(string accountName, int amount, int pin)
        {
            try
            {
                return factory.RaiseALoan(accountName, amount, pin);
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in RaiseALoan: {0}", e.Message);
                return false;
            }
        }
    }
}
