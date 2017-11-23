using Common.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IDS
{
    class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding adminsBinding = new NetTcpBinding();
            string adminsAddress = ConfigurationManager.AppSettings["bankingServicesAddress"];
            ServiceHost adminsSvcHost = new ServiceHost(typeof(BankingServiceIDS));
            adminsSvcHost.AddServiceEndpoint(typeof(IBankingService), adminsBinding, adminsAddress);

            try
            {
                adminsSvcHost.Open();
                Console.WriteLine("IDSServices is opened...");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while trying to open host for admins {e.Message}");
                Console.WriteLine($"[StackTrace] {e.StackTrace}");
                adminsSvcHost.Close();

                return;
            }

            Console.WriteLine("Press any key for exit.");
            Console.ReadLine();
            adminsSvcHost.Close();
        }
    }
}
