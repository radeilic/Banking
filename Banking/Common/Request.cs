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
        private int amount;
        private bool isPayment;

        public DateTime TimeOfCreation
        {
            get { return timeOfCreation; }
            set { timeOfCreation = value; }
        }

        public bool IsPayment
        {
            get { return isPayment; }
            set { isPayment = value; }
        }

        public Account Account
        {
            get { return account; }
            set { account = value; }
        }

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public RequestState State
        {
            get { return state; }
            set { state = value; }
        }


        public Request(DateTime timeOfCreation, Account account, int amount)
        {
            this.TimeOfCreation = timeOfCreation;
            this.Account = account;
            this.State = RequestState.WAIT;
            this.Amount = amount;
        }

        public Request(DateTime timeOfCreation, Account account, int amount, bool isPayment)
        {
            this.TimeOfCreation = timeOfCreation;
            this.Account = account;
            this.State = RequestState.WAIT;
            this.Amount = amount;
            this.IsPayment = isPayment;
        }
        
    }
}
