using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BankingService
{
    class BankingServiceIDSProxy : ChannelFactory<IBankingService>, IBankingService, IDisposable
    {
        IBankingService factory;
        public BankingServiceIDSProxy(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{ 
            factory = this.CreateChannel();
        }

        
        public void Check()
        {
            try
            {
                factory.Check();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in Check: {e.Message}");
            }
        }
    }
}
