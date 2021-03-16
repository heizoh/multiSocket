using System;
using System.IO;
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
            TcpListener Listner = new TcpListener(iEP);
            TcpClient CL = new TcpClient();
            NetworkStream NS;
            int ResSize = 0;
            string Resmsg, AnsMsg;
            byte[] ResByte = new byte[256];

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

            }
        }

        public void AcceptClient(object sender,EventArgs e)
        {

        }
    }
}
