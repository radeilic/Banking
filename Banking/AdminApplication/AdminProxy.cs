using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AdminApplication
{
    class AdminProxy : ChannelFactory<IAdminServices>, IAdminServices, IDisposable
    {
        IAdminServices factory;
        public AdminProxy(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
            factory = this.CreateChannel();
        }

        public bool CheckRequest()
        {
            try
            {
                return factory.CheckRequest();
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
            catch (Exception e)
            {
                Console.WriteLine("Exception in CreateBase: {0}", e.Message);
                return false;
            }
        }


    }
}
