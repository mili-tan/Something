using System;
using System.Runtime.InteropServices;
using WanaKanaSharp;

namespace KanjiTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var hepburnConverter = new HepburnConverter();
            try
            {
                var ifeLang = Activator.CreateInstance(Type.GetTypeFromProgID("MSIME.Japan")) as IFELanguage;
                int hr = ifeLang.Open();
                if (hr != 0)
                    throw Marshal.GetExceptionForHR(hr);

                hr = ifeLang.GetPhonetic("けど想うほどに", 1, -1, out var yomigana);
                if (hr != 0)
                {
                    throw Marshal.GetExceptionForHR(hr);
                }
                Console.WriteLine("Kana:" + yomigana);
                Console.WriteLine("Romaji:" + WanaKana.ToRomaji(hepburnConverter, yomigana));

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