using System;
using System.IO.Ports;
using System.Threading;
using NmeaParser.Messages;

namespace NEMA
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPort serialPort = new SerialPort("COM4", 9600);
            serialPort.Open();
            var openZda = new byte[] { 0x24, 0x45, 0x49, 0x47, 0x50, 0x51, 0x2c, 0x5a, 0x44, 0x41, 0x2a, 0x33, 0x39, 0x0d, 0x0a, 0xb5, 0x62, 0x06, 0x01, 0x03, 0x00, 0xf0, 0x08, 0x01, 0x03, 0x20 };
            serialPort.Write(openZda, 0, openZda.Length);
            Thread.Sleep(1000);
            while (true)
            {
                var str = serialPort.ReadLine().Split('*')[0];
                //Console.WriteLine(str);
                if (!str.Contains("$GPZDA")) continue;
                try
                {
                    var msg = NmeaMessage.Parse(str);
                    if (msg is Zda zda) Console.WriteLine(zda.FixDateTime.ToLocalTime());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
