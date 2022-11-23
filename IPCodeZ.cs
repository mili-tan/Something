using System.Globalization;
using System.Net;

namespace Milkey
{
    internal class IPCodeZ
    {
        public static string ToEmojiString(IPAddress ipAddress)
        {
            var strs = string.Empty;
            foreach (var b in ipAddress.GetAddressBytes())
            {
                var mouse = "🐀".ToCharArray();
                mouse[1] = (char) (mouse[1] + b);
                strs += mouse[0] + "" + mouse[1];
            }
            return strs;
        }

        public static string ToHexString(IPAddress ipAddress)
        {
            return new string(BitConverter.ToString(ipAddress.GetAddressBytes())
                .ToCharArray().Where(x => x != '-').ToArray());
        }

        public static IPAddress FromHexToIp(string ipHex)
        {
            var list = new List<string>();
            for (var i = 0; i < ipHex.Length; i += 2) list.Add(ipHex.Substring(i, 2));
            return new IPAddress(list.Select(x => byte.Parse(x, NumberStyles.HexNumber)).ToArray());
        }

        public static IPAddress FromEmojiToIp(string ipEmoji)
        {
            return new IPAddress(FromEmojiToIpBytes(ipEmoji).ToArray());
        }

        private static IEnumerable<byte> FromEmojiToIpBytes(string ipEmoji)
        {
            var bytes = ipEmoji.ToCharArray().Select(x => (byte)x).ToArray();
            for (var i = 1; i < bytes.Length; i += 2) yield return bytes[i];
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
