using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace security_lab.shared
{
    public class CertificateHelper
    {
        public static RemoteCertificateValidationCallback CustomCertificateValidation(Action<X509Certificate> callback = null)
        {
            var ca = LoadCertificateAuthority();
            return (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                callback?.Invoke(new X509Certificate2(certificate.GetRawCertData()));
                if (sslPolicyErrors == SslPolicyErrors.None) return true; //signed certificate by main-stream CA

                using (var customChain = new X509Chain())
                {
                    customChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                    customChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    customChain.ChainPolicy.ExtraStore.Add(ca);//add our root certificate
                    var result = customChain.Build(certificate as X509Certificate2);
                    return result;
                }
                return false;
            };
        }

        public static X509Certificate2 LoadCertificateAuthority()
        {
            var assembly = Assembly.GetEntryAssembly();
            var resourceName = assembly.GetManifestResourceNames().First(p => p.EndsWith("certificates.ca.cert.pem"));
            var stream = assembly.GetManifestResourceStream(resourceName);
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return new X509Certificate2(data);
        }

        public static X509Certificate LoadCertificate(string name, string password)
        {
            var assembly = Assembly.GetCallingAssembly();
            var location = Path.GetDirectoryName(assembly.Location);
            return new X509Certificate(Path.Combine(location, "certificates", name), password);
        }
    }
}
