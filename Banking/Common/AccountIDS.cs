using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AccountIDS
    {
        private string accountName;
        private int dailyAmount;
        private DateTime currentDay;
        private int loginAttempts;
        private int requestsCount;
        private DateTime? intervalBeginning;

        public string AccountName
        {
            get { return accountName; }
            set { accountName = value; }
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

        public int RequestsCount
        {
            get { return requestsCount; }
            set { requestsCount = value; }
        }

        public DateTime? IntervalBeginning
        {
            get { return intervalBeginning; }
            set { intervalBeginning = value; }
        }


        public AccountIDS(string accountName)
        {
            this.AccountName = accountName;
            this.dailyAmount = 0;
            this.CurrentDay = DateTime.Now.Date;
            this.LoginAttempts = 0;
            this.RequestsCount = 0;
        }
    }
}
