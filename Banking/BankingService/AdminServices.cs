using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService
{
    public class AdminServices : IAdminServices
    {
        public bool CheckRequest()
        {
            Console.WriteLine("CheckRequest called.");
            return true;
        }

        public bool CreateBase()
        {
            Console.WriteLine("CreateBase called.");
            return true;
        }
    }
}
