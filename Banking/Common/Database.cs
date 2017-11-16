using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class Database
    {
        public object accountsLock;
        public object accountRequestsLock;
        public object loansRequestsLock;
        public object paymentsRequestsLock;

        public static List<Account> accounts;
        public static Queue<Request> accountsRequests;
        public static Queue<Request> loansRequests;
        public static Queue<Request> paymentRequests;

    }
}
