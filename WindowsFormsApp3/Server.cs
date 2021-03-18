using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace WindowsFormsApp3
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            IPEndPoint iEP = new IPEndPoint(IPAddress.Any, 39999);
            Task.Run(() => AcceptClient(iEP));
        }

        public void AcceptClient(IPEndPoint iEP)
        {
            TcpListener Listner = new TcpListener(iEP);
            TcpClient CL = new TcpClient();
            NetworkStream NS;
            Encoding Enc = Encoding.ASCII;

            SynchronizationContext _sync = SynchronizationContext.Current;

            int ResSize = 0;
            string Resmsg, AnsMsg;
            byte[] ResByte = new byte[256];
            byte[] Ansbyte = new byte[256];

            MemoryStream MS = new MemoryStream();

            Listner.Start(10);
            while (true)
            {
                CL = Listner.AcceptTcpClient();
                NS = CL.GetStream();
                do
                {
                    ResSize = NS.Read(ResByte, 0, ResByte.Length);
                    MS.Write(ResByte, 0, ResSize);
                } while (ResSize > 0 && ResByte[255] != '\0');

                Resmsg = Enc.GetString(ResByte, 0, ResSize).TrimEnd('\0');

                Array.Clear(ResByte, 0, 256);

                string num = Regex.Replace(Resmsg, @"[^0-9]", "");
                AnsMsg = $"サーバーからクライアント{num}に応答";
                Ansbyte = Encoding.UTF8.GetBytes(AnsMsg);
                NS.Write(Ansbyte, 0, Ansbyte.Length);
                Array.Clear(Ansbyte, 0, Ansbyte.Length);
                Debug.WriteLine($"Server->Client{num} fin");
                heavymethod();
            }

        }

        private void heavymethod()
        {
            Thread.Sleep(10000);
        }
    }
}
