using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;

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

        public IDSResult Check(Request request)
        {
            try
            {
                return factory.Check(request);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in Check: {e.Message}");
                return IDSResult.Exception;
            }
        }
    }
}
