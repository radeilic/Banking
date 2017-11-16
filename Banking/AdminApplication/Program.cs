using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AdminApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:25000/AdminServices";

            using (AdminProxy proxy = new AdminProxy(binding, new EndpointAddress(new Uri(address))))
            {
                proxy.Init();
                proxy.CheckRequest();
            }
            Console.WriteLine("Press any key to close AdminApp.");
            Console.ReadKey(true);
        }
    }
}
