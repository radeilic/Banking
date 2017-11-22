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
        private int dailyAmount;
        [DataMember]
        private DateTime currentDay;
        [DataMember]
        private int pin;
        [DataMember]
        private int loginAttempts;
        [DataMember]
        private int requestsCount;
        [DataMember]
        private DateTime? intervalBeginning;

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
        public int RequestsCount
        {
            get { return requestsCount; }
            set { requestsCount = value; }
        }
        public DateTime? IntevalBeginning
        {
            get { return intervalBeginning; }
            set { intervalBeginning = value; }
        }
        //[DataMember(Name = "intevalBeginning")]
        //private string IntevalBeginningString { get; set; }

        //[OnSerializing]
        //void OnSerializing(StreamingContext context)
        //{
        //    if (this.IntevalBeginning == null)
        //        this.IntevalBeginningString = "";
        //    else
        //        this.IntevalBeginningString = this.IntevalBeginning.Value.ToString("MMM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
        //}

        //[OnDeserialized]
        //void OnDeserializing(StreamingContext context)
        //{
        //    if (this.IntevalBeginningString == "")
        //        this.IntevalBeginning = null;
        //    else
        //        this.IntevalBeginning = DateTime.ParseExact(this.IntevalBeginningString, "MMM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
        //}

        public Account(string owner, string accountName)
        {
            this.Owner = owner;
            this.AccountName = accountName;
            this.Amount = 0;
            this.IsBlocked = false;
            this.dailyAmount = 0;
            this.CurrentDay = DateTime.Now.Date;
            this.LoginAttempts = 0;
            this.RequestsCount = 0;
        }
    }
}
