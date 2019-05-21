using System;
using System.Security.Cryptography.X509Certificates;

namespace mCrt
{
    static class Program
    {
        static void Main()
        {
            X509Certificate2 certificate2 = new X509Certificate2("dst.crt");
            Console.WriteLine(certificate2.IssuerName.Name);
            Console.WriteLine(certificate2.SerialNumber);
            Console.WriteLine(certificate2.Subject);
            Console.WriteLine(certificate2.SignatureAlgorithm.FriendlyName);
            Console.WriteLine(certificate2.Version);
            Console.WriteLine(certificate2.Thumbprint);
            Console.WriteLine(certificate2.Handle);
            Console.WriteLine(certificate2.SubjectName.Name);
            Console.ReadKey();
        }
    }
}
