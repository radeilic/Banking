using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Services;


namespace BankingService
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address1 = "net.tcp://localhost:25000/AdminServices";
            ServiceHost host1 = new ServiceHost(typeof(AdminServices));
            host1.AddServiceEndpoint(typeof(IAdminServices), binding, address1);
            host1.Open();
            Console.WriteLine("AdminServices is opened...");
            
            NetTcpBinding binding2 = new NetTcpBinding();
            string address2 = "net.tcp://localhost:25001/UserServices";
            ServiceHost host2 = new ServiceHost(typeof(UserServices));
            host2.AddServiceEndpoint(typeof(IUserServices), binding2, address2);
            host2.Open();
            Console.WriteLine("UserServices is opened...");

            Console.WriteLine("Press any key to close server.");
            Console.ReadKey();

            host1.Close();
            host2.Close();
        }



    }
}
