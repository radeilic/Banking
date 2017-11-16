using Common.Certifications;
using Common.Services;
using System;
using System.Collections.Generic;
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
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            //this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }
        public bool OpenAccount(string accountName)
        {
            try
            {
                return factory.OpenAccount(accountName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in OpenAccount: {0}",e.Message);
                return false;
            }
            
        }

        public bool Payment(bool isPayment,string accountName, int amount)
        {
            try
            {
                return factory.Payment(isPayment, accountName, amount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Payment: {0}", e.Message);
                return false;
            }
        }

        public bool RaiseALoan(string accountName, int amount)
        {
            try
            {
                return factory.RaiseALoan(accountName, amount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in RaiseALoan: {0}", e.Message);
                return false;
            }
        }
    }
}
