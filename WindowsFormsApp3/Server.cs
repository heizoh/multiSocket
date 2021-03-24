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
		private Class_CommTCP CC_TCP;
		private void Server_Load(object sender, EventArgs e)
		{
			Form fm = new Form();
			TextBox tb = textBox1;
			fm = this;
			IPEndPoint iEP = new IPEndPoint(IPAddress.Any, 39999);
			CC_TCP = new Class_CommTCP(iEP, Encoding.UTF8, fm, tb);
			CC_TCP.SD_Send += signing;
		}



		private void signing(object o, OnEventArgs oea)
        {
			int tnum = oea.tNum;
			int num = oea.Num;
			heavymethod();
			CC_TCP.send(oea.SOC, "test");
			textBox1.AppendText($"【送信 {num:00}】test-{tnum:00}\r\n");
        }
		

		private void heavymethod()
		{
			Thread.Sleep(500);
			Debug.WriteLine("処理終了");
		}
	}
}
