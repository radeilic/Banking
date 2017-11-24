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

        public static EventLog CustomLog = null;
        const string SourceName = "Common.Audit";
        const string LogName = "BankingLog";

        static Audit()
        {
            try
            {
                if (!EventLog.Exists(LogName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                CustomLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while trying to open log file.");
                Console.WriteLine($"[StackTrace] {e.StackTrace}");
                CustomLog = null;
            }
        }


        public static void UserOperationSuccess(string userName, string serviceName)
        {
            string message = String.Format(AuditEvents.UserOperationSuccess, userName, serviceName);
            CustomLog.WriteEntry(message, EventLogEntryType.SuccessAudit);
        }


        public static void AdminOperationSuccess(string serviceName)
        {
            string message = String.Format(AuditEvents.AdminOperationSuccess, serviceName);
            CustomLog.WriteEntry(message, EventLogEntryType.SuccessAudit);
        }

        public static void AdminUserAuthenticationAuthorizationSuccess()
        {
            string message = AuditEvents.AdminUserAuthenticationAuthorizationSuccess;
            CustomLog.WriteEntry(message, EventLogEntryType.SuccessAudit);
        }

        public static void UserOperationFailed(string userName, string serviceName, string reason)
        {
            string message = String.Format(AuditEvents.UserOperationFailed, userName,serviceName,reason);
            CustomLog.WriteEntry(message, EventLogEntryType.Error);
        }

        public static void AdminOperationFailed( string serviceName)
        {
            string message = String.Format(AuditEvents.AdminOperationFailed, serviceName);
            CustomLog.WriteEntry(message, EventLogEntryType.Error);
        }

        public static void AdminUserAuthenticationAuthorizationFailed()
        {
            string message = AuditEvents.AdminUserAuthenticationAuthorizationFailed;
            CustomLog.WriteEntry(message, EventLogEntryType.Error);
        }

        public void Dispose()
        {
            if (CustomLog != null)
            {
                CustomLog.Dispose();
                CustomLog = null;
            }
        }
    }
}
