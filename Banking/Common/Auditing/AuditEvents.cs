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
        UserAuthorizationSuccess = 0,
        UserAuthorizationFailed = 1
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

        public static string UserAuthorizationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserAuthorizationSuccess.ToString());
            }
        }

        public static string UserAuthorizationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserAuthorizationFailed.ToString());
            }
        }
    }
}
