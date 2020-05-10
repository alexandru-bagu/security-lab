using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

namespace security_lab.shared
{
    public class ThiefCertificateHelper
    {
        public static RemoteCertificateValidationCallback CustomCertificateValidation()
        {
            var ca = CertificateHelper.LoadCertificateAuthority();
            return (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                return true;
            };
        }
    }
}
