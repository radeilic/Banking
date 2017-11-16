using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:25001/UserServices";

            using (UserProxy proxy = new UserProxy(binding, new EndpointAddress(new Uri(address))))
            {
                proxy.OpenAccount();
                proxy.RaiseALoan();
                proxy.Payment();
            }
            Console.WriteLine("Press any key to close UserApp.");
            Console.ReadKey(true);
        }
    }
}
