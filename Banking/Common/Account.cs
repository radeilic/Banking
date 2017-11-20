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
        private bool isBlocked;
        private DateTime blockedUntil;
        private int dailyAmount;
        private DateTime currentDay;
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
        public int DailyAmount
        {
            get { return dailyAmount; }
            set { dailyAmount = value; }
        }

        public DateTime CurrentDay
        {
            get { return currentDay; }
            set { currentDay = value; }
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
            this.IsBlocked = false;
            this.dailyAmount = 0;
            this.CurrentDay = DateTime.Now.Date;
            this.LoginAttempts = 0;
        }
    }
}
