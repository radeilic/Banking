using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Request
    {
        [DataMember]
        private DateTime timeOfCreation;
        [DataMember]
        private Account account;
        [DataMember]
        private int pin;
        [DataMember]
        private RequestState state;
        [DataMember]
        private int amount;
        [DataMember]
        private RequestType type;
        [DataMember]
        private bool isOutgoing;
        
        public DateTime TimeOfCreation
        {
            get { return timeOfCreation; }
            set { timeOfCreation = value; }
        }

        public RequestType Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool IsOutgoing
        {
            get { return isOutgoing; }
            set { isOutgoing = value; }
        }

        public Account Account
        {
            get { return account; }
            set { account = value; }
        }

        public int PIN
        {
            get { return pin; }
            set { pin = value; }
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


        public Request(RequestType type,DateTime timeOfCreation, Account account, int amount)
        {
            this.Type = type;
            this.TimeOfCreation = timeOfCreation;
            this.Account = account;
            this.State = RequestState.WAIT;
            this.Amount = amount;
        }

        public Request(RequestType type, DateTime timeOfCreation, Account account, int pin, int amount)
        {
            this.Type = type;
            this.TimeOfCreation = timeOfCreation;
            this.Account = account;
            this.PIN = pin;
            this.State = RequestState.WAIT;
            this.Amount = amount;
        }

        public Request(DateTime timeOfCreation, Account account, int pin, int amount, bool isOutgoing)
        {
            this.Type = RequestType.Payment;
            this.TimeOfCreation = timeOfCreation;
            this.Account = account;
            this.PIN = pin;
            this.State = RequestState.WAIT;
            this.Amount = amount;
            this.IsOutgoing = isOutgoing;

        }
        
    }
}
