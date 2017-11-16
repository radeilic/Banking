using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Account
    {
        string owner;
        string accountName;

        public string Owner { get; set; }

        public string AccountName { get; set; }

        public Account(string owner, string accountName)
        {
            this.Owner = owner;
            this.AccountName = accountName;
        }
    }
}
