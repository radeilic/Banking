using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
            factory = this.CreateChannel();
        }
        public bool OpenAccount()
        {
            try
            {
                return factory.OpenAccount();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in OpenAccount: {0}",e.Message);
                return false;
            }
            
        }

        public bool Payment()
        {
            try
            {
                return factory.Payment();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Payment: {0}", e.Message);
                return false;
            }
        }

        public bool RaiseALoan()
        {
            try
            {
                return factory.RaiseALoan();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in RaiseALoan: {0}", e.Message);
                return false;
            }
        }
    }
}
