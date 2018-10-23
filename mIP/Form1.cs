using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mIP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            IPAddress[] IPs = Dns.GetHostAddresses(textBox1.Text);
            foreach (IPAddress ip in IPs)
            {
                listBox1.Items.Add(ip.ToString());
            }
        }
    }
}
