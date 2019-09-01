using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using WanaKanaSharp;

namespace KanjiTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var words = File.ReadAllLines(args.Length == 0 ? "words.txt" : args[0]);

            var hepburnConverter = new HepburnConverter();
            List<string> kanaStrings = new List<string>();
            List<string> romajiStrings = new List<string>();
            try
            {
                var ifeLang = Activator.CreateInstance(Type.GetTypeFromProgID("MSIME.Japan")) as IFELanguage;
                int hr = ifeLang.Open();
                if (hr != 0)
                    throw Marshal.GetExceptionForHR(hr);

                foreach (var item in words)
                {
                    hr = ifeLang.GetPhonetic(item, 1, -1, out var yomigana);
                    if (hr != 0)
                    {
                        throw Marshal.GetExceptionForHR(hr);
                    }

                    if (string.IsNullOrWhiteSpace(item))
                    {
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Origin:" + item);
                        Console.WriteLine("Kana:" + yomigana);
                        var ganaSpace = "";
                        foreach (var word in yomigana) ganaSpace += word + " ";
                        Console.WriteLine("Romaji:" + WanaKana.ToRomaji(hepburnConverter, ganaSpace));
                        kanaStrings.Add(yomigana);
                        romajiStrings.Add(WanaKana.ToRomaji(hepburnConverter, ganaSpace));
                    }
                }

                File.AppendAllLines("kana.txt",kanaStrings);
                File.AppendAllLines("romaji.txt", romajiStrings);

                Console.WriteLine("----------------");
                Console.WriteLine("Done!");
                Console.ReadLine();
            }
            catch (COMException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    // IFELanguage2 Interface ID
    //[Guid("21164102-C24A-11d1-851A-00C04FCC6B14")]
    [ComImport]
    [Guid("019F7152-E6DB-11d0-83C3-00C04FDDB82E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFELanguage
    {
        int Open();
        int Close();
        int GetJMorphResult(uint dwRequest, uint dwCMode, int cwchInput, [MarshalAs(UnmanagedType.LPWStr)] string pwchInput, IntPtr pfCInfo, out object ppResult);
        int GetConversionModeCaps(ref uint pdwCaps);
        int GetPhonetic([MarshalAs(UnmanagedType.BStr)] string @string, int start, int length, [MarshalAs(UnmanagedType.BStr)] out string result);
        int GetConversion([MarshalAs(UnmanagedType.BStr)] string @string, int start, int length, [MarshalAs(UnmanagedType.BStr)] out string result);
    }
}