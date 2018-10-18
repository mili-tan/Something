using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Playground_Fjedu;

namespace FJEDU_FUNNN
{
    public partial class Form1 : Form
    {
        private string[] UA = {
            "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",
            "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11) like Gecko",
            "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11) like Gecko",
            "Mozilla/5.0 (compatible; MSIE 9.0; AOL 9.7; AOLBuild 4343.19; Windows NT 6.1; WOW64; Trident/5.0; FunWebProducts)",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.32 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.84 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1284.0 Safari/537.13",
            "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 Edge/16.16299",
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.4 (KHTML, like Gecko) Maxthon/3.0.6.27 Safari/532.4"
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxSpace.Text = File.ReadAllText("space.text");
            listBoxUser.Items.AddRange(File.ReadAllLines("users.text"));

            progressBar1.Maximum = listBoxUser.Items.Count;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var countVar = 0;
            foreach (var item in listBoxUser.Items)
            {
                countVar++;
                string user = JsEx.ExecuteJScript($@"encrypt(""{item}"")", File.ReadAllText("encrypt.js"));
                string ps = JsEx.ExecuteJScript(@"encrypt(""12345678"")", File.ReadAllText("encrypt.js"));
                string spaceId = textBoxSpace.Text;

                string urlLogin =
                    @"http://www.fjedu.cn/index.php?r=portal/user/doLoginNew&ajax=1&callback=jQuery17209242679668427426_1539562717646&userId=" + user + "&userPsw=" + ps + "&remember=1&vaildata=auaZtr1%252FbAX0iiYBsWS9AkjZeNv3gCXOlRRr%252FDyCpVvteAhjaDI4&valCode=&refer=center&_=1539563257379";
                var urlSpace =
                    @"http://www.fjedu.cn/index.php?r=space/person/index&sid=" + spaceId;

                string s1 = @"http://www.fjedu.cn" + "/index.php?r=center/person/message/Pull&callback=";
                string s2 = @"http://www.fjedu.cn" + "/index.php?r=space/person/visitor/index&sid=" + spaceId;
                string s3 = @"http://www.fjedu.cn" + "/index.php?r=space/person/index/GetModuleHtml&wname=&name=postCategory&spacetype=space_person&sid=" + spaceId;
                string s4 = @"http://www.fjedu.cn" + "/index.php?r=portal/Vcode/GetNewCode&callback=";
                string s5 = @"http://www.fjedu.cn" + "/index.php?r=space/person/index/GetModuleHtml&wname=&name=visitor&spacetype=space_person&sid=" + spaceId;
                string s6 = @"http://www.fjedu.cn" + "/index.php?r=center/person/message/getUnreadCount&callback=";
                string s7 = @"http://www.fjedu.cn" + "/index.php?r=regSiteDomain/Reg&callback=regSiteDomainEnd&url=http%3A%2F%2Fwww.fjedu.cn&_=1539690788026";
                string s8 = @"http://www.fjedu.cn" + "/index.php?r=space/person/index/GetModuleHtml&wname=&name=visitor&spacetype=space_person&sid=" + spaceId;
                string urlLoginOut = @"http://www.fjedu.cn" + "/index.php?r=uc/site";

                Console.WriteLine(@"--------登录--------");
                using (WebR.HttpClient httpClient = new WebR.HttpClient { Cookies = new CookieContainer() })
                {
                    var mua = UA[new Random(DateTime.Now.Second).Next(0, UA.Length - 1)];

                    httpClient.Headers["User-Agent"] = mua;
                    Console.WriteLine(httpClient.DownloadString(urlLogin));
                    Console.WriteLine(@"--------Cookie--------");
                    var cookieStr = "";
                    foreach (var cookie in httpClient.Cookies.GetCookies(new Uri("http://www.fjedu.cn/")))
                    {
                        cookieStr += cookie + "; ";
                    }
                    Debug.WriteLine(cookieStr);
                    Debug.WriteLine("-------访问空间-------");

                    var webc = new WebClient();

                    webc.Headers["Cookie"] = cookieStr;
                    webc.Headers["User-Agent"] = mua;
                    httpClient.Headers["Cookie"] = cookieStr;
                    string s = webc.DownloadString(urlSpace);
                    Debug.Write(s);

                    Debug.WriteLine("-------GET请求（1）-------");
                    Debug.WriteLine(webc.DownloadString(s1));
                    Debug.WriteLine("-------GET请求（2）-------");
                    Debug.WriteLine(webc.DownloadString(s2));
                    Debug.WriteLine("-------GET请求（3）-------");
                    Debug.WriteLine(webc.DownloadString(s3));
                    Debug.WriteLine("-------GET请求（4）-------");
                    Debug.WriteLine(webc.DownloadString(s4));
                    Debug.WriteLine("-------GET请求（5）-------");
                    Debug.WriteLine(webc.DownloadString(s5));
                    Debug.WriteLine("-------GET请求（6）-------");
                    Debug.WriteLine(webc.DownloadString(s6));
                    Debug.WriteLine("-------GET请求（7）-------");
                    Debug.WriteLine(webc.DownloadString(s7));
                    Debug.WriteLine("-------GET请求（8）-------");
                    Debug.WriteLine(webc.DownloadString(s8));
                    Debug.WriteLine("-------访问成功-------");
                    Debug.WriteLine(webc.DownloadString(urlLoginOut));
                    Debug.WriteLine("-------登出成功-------");
                }
                backgroundWorker.ReportProgress(countVar);
                Thread.Sleep(new Random(DateTime.Now.Second).Next(1, 3) * 100);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            Text = $@"{progressBar1.Value}/{progressBar1.Maximum}";
        }
    }
}
