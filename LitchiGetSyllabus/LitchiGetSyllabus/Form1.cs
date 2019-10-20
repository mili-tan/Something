using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LitchiGetSyllabus
{
    public partial class Form1 : Form
    {
        public static WebBrowser MWebBrowser = new WebBrowser();
        public static string PHPSESSID;
        public static string PHPSESSID_NS_Sig;
        public static string IP = IPAddress.Parse(File.ReadAllText("ip.text")).ToString();

        private const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            int flags,
            IntPtr pReserved);

        public static string GetCookie(string url)
        {
            int size = 512;
            StringBuilder strBuilder = new StringBuilder(size);
            if (InternetGetCookieEx(url, null, strBuilder, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                return strBuilder.ToString();
            if (size < 0)
                return null;
            strBuilder = new StringBuilder(size);
            return !InternetGetCookieEx(url, null, strBuilder, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero) ? null : strBuilder.ToString();
        }

        public Form1()
        {
            InitializeComponent();
            MWebBrowser.Url = new Uri($"http://{IP}/studentportal.php/");
            MWebBrowser.DocumentCompleted += (sender, args) =>
            {
                if (MWebBrowser.Url.ToString() == $"http://{IP}/studentportal.php/Main")
                {
                    var c1 = GetCookie(MWebBrowser.Url.ToString()).Split(';');
                    PHPSESSID = c1[0].Split('=')[1];
                    PHPSESSID_NS_Sig = c1[1].Split('=')[1];
                    MWebBrowser.Stop();
                    MWebBrowser.Hide();
                    button1.Enabled = true;
                }
            };
            Controls.Add(MWebBrowser);
            MWebBrowser.Dock = DockStyle.Fill;
            MWebBrowser.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.ProgressChanged += (o, args) => { progressBar1.Value = args.ProgressPercentage; };
            var index = 1;
            bgWorker.DoWork += (o, args) =>
            {
                foreach (var itemLine in File.ReadAllLines("./list.text"))
                {
                    var item = itemLine.Split(',');
                    var img = WebSnapshotsHelper.GetWebSiteThumbnail($"http://{IP}/studentportal.php/" +
                                                                     $"Jxxx/xskbxx/optype/2/xn/{yr1.Text}-{yr2.Text}/xq/{semester.Text}/dqz/{week.Text}/" +
                                                                     $"sybmdmstr/{item[0]}/bjmc/{item[1]}"
                        , 1200, 600, 1200, 600);

                    Bitmap bitmap = new Bitmap(img);
                    if (MakeTransparent.Checked) bitmap.MakeTransparent(Color.White);
                    bitmap.Save($"{item[1]}.jpg");
                    bgWorker.ReportProgress(index++);
                    Thread.Sleep(1000);
                }

                MessageBox.Show(@"done!");
            };
            bgWorker.RunWorkerAsync();
        }

        public class WebSnapshotsHelper
        {
            [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

            Bitmap mBitmap;
            readonly string mUrl;
            int mBrowserWidth, mBrowserHeight, mThumbnailWidth, mThumbnailHeight;
            public WebSnapshotsHelper(string Url, int BrowserWidth, int BrowserHeight, int ThumbnailWidth, int ThumbnailHeight)
            {
                mUrl = Url;
                mBrowserHeight = BrowserHeight;
                mBrowserWidth = BrowserWidth;
                mThumbnailWidth = ThumbnailWidth;
                mThumbnailHeight = ThumbnailHeight;
            }
            public static Bitmap GetWebSiteThumbnail(string Url, int BrowserWidth, int BrowserHeight, int ThumbnailWidth, int ThumbnailHeight)
            {
                WebSnapshotsHelper thumbnailGenerator = new WebSnapshotsHelper(Url, BrowserWidth, BrowserHeight, ThumbnailWidth, ThumbnailHeight);
                return thumbnailGenerator.GenerateWebSiteThumbnailImage();
            }
            public Bitmap GenerateWebSiteThumbnailImage()
            {
                Thread mThread = new Thread(Generate_WebSiteThumbnailImage);
                mThread.SetApartmentState(ApartmentState.STA);
                mThread.Start();
                mThread.Join();
                return mBitmap;
            }
            private void Generate_WebSiteThumbnailImage()
            {
                WebBrowser mWebBrowser = new WebBrowser();
                InternetSetCookie(mUrl, "PHPSESSID", PHPSESSID);
                InternetSetCookie(mUrl, "PHPSESSID_NS_Sig", PHPSESSID_NS_Sig);
                mWebBrowser.ScrollBarsEnabled = false;
                mWebBrowser.Navigate(mUrl);
                mWebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
                while (mWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                    Application.DoEvents();
                mWebBrowser.Dispose();
            }
            private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
            {
                WebBrowser mWebBrowser = (WebBrowser)sender;
                mWebBrowser.ClientSize = new Size(mBrowserWidth, mBrowserHeight);
                mWebBrowser.ScrollBarsEnabled = false;
                mBitmap = new Bitmap(mWebBrowser.Bounds.Width, mWebBrowser.Bounds.Height);
                mWebBrowser.BringToFront();
                mWebBrowser.DrawToBitmap(mBitmap, mWebBrowser.Bounds);
                mBitmap = (Bitmap)mBitmap.GetThumbnailImage(mThumbnailWidth, mThumbnailHeight, null, IntPtr.Zero);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var listLines = File.ReadAllLines("./list.text");
            listBox1.Items.AddRange(listLines);
            progressBar1.Maximum = listLines.Length;
        }
    }
}
