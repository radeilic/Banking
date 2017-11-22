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
        AdminOperationSuccess = 4,
        AdminUserAuthenticationAuthorizationSuccess = 5,
        AdminUserAuthenticationAuthorizationFailed = 6
    }

    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMamager
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
                return ResourceMamager.GetString(AuditEventTypes.UserOperationFailed.ToString());
            }
        }

        public static string UserOperationSuccess
        {
            get
            {
                return ResourceMamager.GetString(AuditEventTypes.UserOperationSuccess.ToString());
            }
        }

        public static string AdminOperationFailed
        {
            get
            {
                return ResourceMamager.GetString(AuditEventTypes.AdminOperationFailed.ToString());
            }
        }

        public static string AdminOperationSuccess
        {
            get
            {
                return ResourceMamager.GetString(AuditEventTypes.AdminOperationSuccess.ToString());
            }
        }

        public static string AdminUserAuthenticationAuthorizationSuccess
        {
            get
            {
                return ResourceMamager.GetString(AuditEventTypes.AdminUserAuthenticationAuthorizationSuccess.ToString());
            }
        }

        public static string AdminUserAuthenticationAuthorizationFailed
        {
            get
            {
                return ResourceMamager.GetString(AuditEventTypes.AdminUserAuthenticationAuthorizationFailed.ToString());
            }
        }
    }
}
