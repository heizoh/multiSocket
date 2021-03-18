using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace WindowsFormsApp3
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        private readonly int _port = 39999;

        private void Client_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int n = (int)numericUpDown1.Value;
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"),_port);
            TcpClient[] CL = new TcpClient[n];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                CL[i] = new TcpClient();
            }

            Parallel.For(0, n, h =>
             {
                 Debug.WriteLine($"{h:00} -> Start!");
                 CL[h].Connect(EP.Address,EP.Port);
                 using(var stream = CL[h].GetStream())
                 {
                     for (int i = 0; i < 2; i++)
                     {
                         string msg = $"クライアント{h:00}からサーバーへメッセージ送信 {i+1}回目";
                         byte[] sendByte = Encoding.UTF8.GetBytes(msg);
                         stream.Write(sendByte, 0, sendByte.Length);
                         sb.AppendLine($"C{h:00}送信：{msg}");
                         byte[] retByte = new byte[256];
                         stream.Read(retByte, 0, 256);
                         string ret = Encoding.UTF8.GetString(retByte, 0, retByte.Length).TrimEnd('\0');
                         sb.AppendLine($"C{h:00}受信：{ret}");

                     }
                 }
                 Debug.WriteLine($"{h:00} -> Fin?");

             });
            textBox1.Text = sb.ToString();
            this.Refresh();

        }
    }
}
