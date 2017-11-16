using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Request
    {
        private DateTime timeOfCreation;
        private Account account;

        public DateTime TimeOfCreation
        {
            get { return timeOfCreation; }
            set { timeOfCreation = value; }
        }
        
        public Account Account
        {
            get { return account; }
            set { account = value; }
        }

        public Request(DateTime timeOfCreation, Account account)
        {
            this.TimeOfCreation = timeOfCreation;
            this.Account = account;
        }
    }
}
