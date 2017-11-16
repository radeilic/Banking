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
        private RequestState state;

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

        public RequestState State
        {
            get { return state; }
            set { state = value; }
        }


        public Request(DateTime timeOfCreation, Account account)
        {
            this.TimeOfCreation = timeOfCreation;
            this.Account = account;
            this.State = RequestState.WAIT;
        }
    }
}
