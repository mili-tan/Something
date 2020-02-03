using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WindowsHello;
using OtpNet;

namespace UWApi
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //var handle = new IntPtr();
            //var data = Encoding.UTF8.GetBytes("test");
            //var provider = WinHelloProvider.CreateInstance("使用 Windows Hello 来解锁。", handle);
            //var encryptedData = provider.Encrypt(data);
            //var decryptedData = provider.PromptToDecrypt(encryptedData);

            //File.WriteAllBytes("./dat.dat",encryptedData);

            //MessageBox.Show(Convert.ToBase64String(encryptedData));
            //MessageBox.Show(Encoding.UTF8.GetString(decryptedData));
            //this.Close();

            var totp = new Totp(Base32Encoding.ToBytes("2LKNGZKXQCDHKDHJ"));
            MessageBox.Show(totp.ComputeTotp(DateTime.UtcNow));
            Clipboard.SetText(totp.ComputeTotp(DateTime.UtcNow));
            this.Close();
        }
    }
}
