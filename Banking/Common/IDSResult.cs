using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum IDSResult
    {
        OK,
        FailedLoan,
        FailedPayment,
        BlockForOverload,
        BlockForWrongPIN,
        BlockForDailyLimit,
        Exception=-1
    }
}
