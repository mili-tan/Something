using System;
using Nancy.Hosting.Self;

namespace NancyTest
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Once Data Storage API");

            using (NancyHost host = new NancyHost(new HostConfiguration
                    {RewriteLocalhost = true, UrlReservations = new UrlReservations {CreateAutomatically = true}},
                    new Uri("http://localhost:11108/")))
            {
                host.Start();

                Console.WriteLine("Data Storage API is running on 11108 port");
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }
}
