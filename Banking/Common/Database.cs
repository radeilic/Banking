using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class Database
    {
        public static object AccountsLock = new object();
        public static object AccountRequestsLock = new object();
        public static object LoanRequestsLock = new object();
        public static object PaymentRequestsLock = new object();

        public static Dictionary<string, Account> Accounts;
        public static List<Request> AccountRequests;
        public static List<Request> LoanRequests;
        public static List<Request> PaymentRequests;

    }
}
