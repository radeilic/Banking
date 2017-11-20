using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Common.Auditing
{
    public enum AuditEventTypes
    {
        UserOperationFailed = 0,
        UserOperationSuccess = 1,
        AdminOperationFailed = 2,
        AdminOperationSuccess = 4
    }

    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null)
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventsFile).FullName, Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string UserOperationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserOperationFailed.ToString());
            }
        }

        public static string UserOperationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserOperationSuccess.ToString());
            }
        }

        public static string AdminOperationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.AdminOperationFailed.ToString());
            }
        }

        public static string AdminOperationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.AdminOperationSuccess.ToString());
            }
        }
    }
}
