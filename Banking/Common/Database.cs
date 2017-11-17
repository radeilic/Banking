using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class Database
    {
        public static object accountsLock = new object();
        public static object accountRequestsLock = new object();
        public static object loansRequestsLock = new object();
        public static object paymentsRequestsLock = new object();

        public static Dictionary<string, Account> accounts;
        public static List<Request> accountsRequests;
        public static List<Request> loansRequests;
        public static List<Request> paymentRequests;

    }
}
