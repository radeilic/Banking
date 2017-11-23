using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Certifications
{
    public class Formatter
    {
        public static string ParseName(string winLogonName)
        {
            string[] parts;

            if (winLogonName.Contains("@"))
            {
                parts = winLogonName.Split('@');
                return parts[0];
            }

            if (winLogonName.Contains("\\"))
            {
                parts = winLogonName.Split('\\');
                return parts[1];
            }
            
            return winLogonName;
        }

        public static string FormatName(string name)
        {
            return $"{Environment.MachineName}\\{name}";
        }
    }
}
