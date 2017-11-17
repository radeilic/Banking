using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class Database
    {
        public static object accountsLock;
        public static object accountRequestsLock;
        public static object loansRequestsLock;
        public static object paymentsRequestsLock;

        public static Dictionary<string, Account> accounts;
        public static List<Request> accountsRequests;
        public static List<Request> loansRequests;
        public static List<Request> paymentRequests;

    }
}
