using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common.Certifications
{
    public class CertManager
    {
       public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

            foreach (X509Certificate2 certificate in certCollection)
            {
                if (certificate.SubjectName.Name.Equals($"CN={subjectName}"))
                {
                    return certificate;
                }
            }

            return null;
        }
    }
}
