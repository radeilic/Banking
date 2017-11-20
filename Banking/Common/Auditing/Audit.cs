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

        public static EventLog customLog = null;
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
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while trying to open log file.");
                Console.WriteLine($"[StackTrace] {e.StackTrace}");
                customLog = null;
            }
        }


        public static void UserOperationSuccess(string userName, string serviceName)
        {
            string message = String.Format(AuditEvents.UserOperationSuccess, userName, serviceName);
            EventInstance ei = new EventInstance(1, 1, EventLogEntryType.SuccessAudit);
            customLog.WriteEntry(message, EventLogEntryType.SuccessAudit);
        }


        public static void AdminOperationSuccess(string serviceName)
        {
            string message = String.Format(AuditEvents.AdminOperationSuccess, serviceName);
            EventInstance ei = new EventInstance(1, 1, EventLogEntryType.SuccessAudit);
            customLog.WriteEntry(message, EventLogEntryType.SuccessAudit);
        }


        public static void UserOperationFailed(string userName, string serviceName, string reason)
        {
            string message = String.Format(AuditEvents.UserOperationFailed, userName,serviceName,reason);
            EventInstance ei = new EventInstance(1, 1, EventLogEntryType.Error);
            customLog.WriteEntry(message, EventLogEntryType.Error);
        }


        public static void AdminOperationFailed( string serviceName)
        {
            string message = String.Format(AuditEvents.AdminOperationFailed, serviceName);
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
