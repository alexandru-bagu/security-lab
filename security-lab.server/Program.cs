using security_lab.shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace security_lab.server
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverCrt = CertificateHelper.LoadCertificate("server.pfx", "server");

            TcpListener listener = new TcpListener(IPAddress.Any, Constants.Port);
            listener.Start(1);
            while (true)
            {
                X509Certificate2 remoteCertificate = null;
                using (var tcpClient = listener.AcceptTcpClient())
                using (var stream = tcpClient.GetStream())
                using (var sslStream = new SslStream(stream, false, CertificateHelper.CustomCertificateValidation((certificate) => remoteCertificate = certificate as X509Certificate2)))
                {
                    try
                    {
                        sslStream.AuthenticateAsServer(serverCrt, true, true);

                        var clientCrt = sslStream.RemoteCertificate as X509Certificate2;
                        using (var writer = new StreamWriter(sslStream))
                        using (var reader = new StreamReader(sslStream))
                        {
                            writer.WriteLine("Hi!");
                            writer.Flush();
                            Console.WriteLine(clientCrt.GetNameInfo(X509NameType.SimpleName, false) + " says " + reader.ReadLine());
                        }
                    }
                    catch (Exception ex)
                    {
                        if (remoteCertificate != null)
                            Console.WriteLine(remoteCertificate.Subject + " signed by " + remoteCertificate.Issuer + " - not valid!");
                        else
                            Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
