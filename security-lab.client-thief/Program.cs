﻿using security_lab.shared;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace security_lab.client
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientCrt = CertificateHelper.LoadCertificate("client.pfx", "client");
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect("localhost", Constants.Port);
                using (var stream = tcpClient.GetStream())
                using (var sslStream = new SslStream(stream, false, ThiefCertificateHelper.CustomCertificateValidation()))
                {
                    sslStream.AuthenticateAsClient("server", new X509CertificateCollection(new[] { clientCrt }), true);
                    if (sslStream.IsAuthenticated && sslStream.IsEncrypted)
                    {
                        var serverCrt = sslStream.RemoteCertificate as X509Certificate2;
                        using (var writer = new StreamWriter(sslStream))
                        using (var reader = new StreamReader(sslStream))
                        {
                            writer.WriteLine("Hi!");
                            writer.Flush();
                            Console.WriteLine(serverCrt.GetNameInfo(X509NameType.SimpleName, false) + " says " + reader.ReadLine());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Server rejected me.");
                    }
                }
            }
        }
    }
}
