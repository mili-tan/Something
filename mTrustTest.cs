using System;
using System.IO;
using System.Net;
using PgpCore;

namespace mTrustME
{
    class Program
    {
        static void Main(string[] args)
        {
            var pgp = new PGP();
            new WebClient().DownloadFile("https://github.com/mili-tan.gpg", "mili-tan.asc");
            var verify = pgp.VerifyFile("linux-arm64.zip.gpg", "mili-tan.asc");
            Console.WriteLine(verify);
        }
    }
}
