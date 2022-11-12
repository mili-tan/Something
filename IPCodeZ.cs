using System.Globalization;
using System.Net;

namespace Milkey
{
    internal class IPZCode
    {
        public static string ToHexString(IPAddress ipAddress)
        {
            return new string(BitConverter.ToString(ipAddress.GetAddressBytes())
                .ToCharArray().Where(x => x != '-').ToArray());
        }

        public static IPAddress FromHexToIp(string ipHex)
        {
            var sList = new List<string>();
            for (var i = 0; i < ipHex.Length; i += 2) sList.Add(ipHex.Substring(i, 2));
            return new IPAddress(sList.Select(b => byte.Parse(b, NumberStyles.HexNumber)).ToArray());
        }

        //public static string ToBase32String(IPAddress ipAddress)
        //{
        //    return Base32.ToString(ipAddress.GetAddressBytes()).TrimEnd('=');
        //}

        //public static IPAddress FromBase32ToIp(string ipBase32)
        //{
        //    return new IPAddress(Base32.ToBytes(ipBase32.PadRight(8, '=')));
        //}
    }
}
