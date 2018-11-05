using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MaterialSkin;

namespace mTomato
{
    public partial class Form1 : MaterialForm
    {

        Icon ico = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        public Form1()
        {
            InitializeComponent();

            MaximizeBox = false;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red800, Primary.Red900, Primary.Green500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            notifyIcon.Icon = ico;
            notifyIcon.Visible = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;

            notifyIcon.ShowBalloonTip(5000, "mTomato - 番茄！", "番茄钟已开始", ToolTipIcon.None);
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            menuItemStart.Enabled = false;
            menuItemStop.Enabled = true;

            ProgressBarTime.Maximum = 1500000;
            timerShow.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            notifyIcon.ShowBalloonTip(5000, "mTomato - 番茄！", "25分钟工作时间已结束 开始休息吧！", ToolTipIcon.None);
            timerRset.Enabled = true;
        }

        private void timerRset_Tick(object sender, EventArgs e)
        {
            timerRset.Enabled = true;
            notifyIcon.ShowBalloonTip(5000, "mTomato - 番茄！", "5分钟休息时间已结束 开始工作吧！", ToolTipIcon.None);
            timer.Enabled = false;

            timerShow.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            notifyIcon.ShowBalloonTip(5000, "mTomato - 番茄！", "番茄钟已停止", ToolTipIcon.None);
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            menuItemStart.Enabled = true;
            menuItemStop.Enabled = false;

            timeValue = 0;
            timerRset.Enabled = false;
            timer.Enabled = false;
        }

        int timeValue = 1500000;
        private void timerShow_Tick(object sender, EventArgs e)
        {
            if (timeValue == 0)
            {
                timeValue = 1500000;
                timerShow.Enabled = false;
            }

            timeValue = timeValue - 1000;
            ProgressBarTime.Value = timeValue;
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                Activate();
                ShowInTaskbar = true;
                notifyIcon.Visible = false;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Icon = ico;
                ShowInTaskbar = false;
                notifyIcon.Visible = true;
            }
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Application.ExitThread();
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("mTomato 1.0.0\n\rCopyright © 2014~2017 Mili Tan. All Rights Reserved. \n\ricon by Aowxtz\n\r感谢使用！");
        }

        private void menuItemStop_Click(object sender, EventArgs e)
        {
            btnStop_Click(null, null);
        }

        private void menuItemStart_Click(object sender, EventArgs e)
        {
            btnStart_Click(null, null);
        }
    }
}
