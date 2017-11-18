using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Auditing
{
    public class Audit:IDisposable
    {

        private static EventLog customLog = null;
        const string SourceName = "SecurityManager.Audit";
        const string LogName = "BankingLog";

        /// <summary>
        /// HOW TO LOG
        /// Audit.AuthorizationSuccess(principal.Identity.Name, "Manage Network Model");
        /// Audit.AuthorizationSuccess(principal.Identity.Name, OperationContext.Current.IncomingMessageHeaders.Action);
        /// </summary>

        static Audit()
        {
            try
            {
                if (!EventLog.Exists(LogName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);

                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
            }
        }

        public static void AuthorizationSuccess(string userName, string serviceName)
        {
            string message = String.Format(AuditEvents.UserAuthorizationSuccess, userName, serviceName);

            EventInstance ei = new EventInstance(1, 1, EventLogEntryType.SuccessAudit);

            customLog.WriteEntry(message, EventLogEntryType.SuccessAudit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="serviceName"> should be read from the OperationContext as follows: OperationContext.Current.IncomingMessageHeaders.Action</param>
        /// <param name="reason">permission name</param>
        public static void AuthorizationFailed(string userName, string serviceName, string reason)
        {
            string message = String.Format(AuditEvents.UserAuthorizationFailed, userName,serviceName,reason);

            EventInstance ei = new EventInstance(1, 1, EventLogEntryType.Error);

            customLog.WriteEntry(message, EventLogEntryType.Error);
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
