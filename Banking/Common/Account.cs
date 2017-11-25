using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Account
    {
        [DataMember]
        private string owner;
        [DataMember]
        private string accountName;
        [DataMember]
        private int amount;
        [DataMember]
        private bool isBlocked;
        [DataMember]
        private DateTime blockedUntil;
        [DataMember]
        private int pin;

        public string  Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        public int PIN
        {
            get { return pin; }
            set { pin = value; }
        }
        public string AccountName
        {
            get { return accountName; }
            set { accountName = value; }
        }
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public bool IsBlocked
        {
            get { return isBlocked; }
            set { isBlocked = value; }
        }
        public DateTime BlockedUntil
        {
            get { return blockedUntil; }
            set { blockedUntil = value; }
        }

        public Account(string owner, string accountName)
        {
            this.Owner = owner;
            this.AccountName = accountName;
            this.Amount = 0;
            this.IsBlocked = false;
        }
    }
}
