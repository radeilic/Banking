using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Account
    {
        private string owner;
        private string accountName;
        private int amount;
        private int pin;
        private int loginAttempts;

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

        public int LoginAttempts
        {
            get { return loginAttempts; }
            set { loginAttempts = value; }
        }

        public Account(string owner, string accountName)
        {
            this.Owner = owner;
            this.AccountName = accountName;
            this.Amount = 0;
            this.LoginAttempts = 0;
        }
    }
}
