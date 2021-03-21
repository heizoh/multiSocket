﻿using System;
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
			Form fm = new Form();
			TextBox tb = textBox1;
			fm = this;
			IPEndPoint iEP = new IPEndPoint(IPAddress.Any, 39999);
			Class_CommTCP CC_TCP = new Class_CommTCP(iEP, Encoding.UTF8, fm, tb);
		}

		public void AcceptClient(IPEndPoint iEP)
		{
			TcpListener Listner = new TcpListener(iEP);
			TcpClient CL = new TcpClient();
			NetworkStream NS;
			Encoding Enc = Encoding.UTF8;

			SynchronizationContext _sync = SynchronizationContext.Current;

			int ResSize = 0;
			string Resmsg, AnsMsg;
			byte[] ResByte = new byte[256];
			byte[] Ansbyte = new byte[256];
			Match[] nums;

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
				this.Invoke((MethodInvoker)(() => textBox1.AppendText(Resmsg + "\r\n")));
				Array.Clear(ResByte, 0, 256);

				MatchCollection Matches = Regex.Matches(Resmsg, "[0-9]{1,2}");
				nums = new Match[Matches.Count];
				Matches.CopyTo(nums, 0);

				int d = Thread.CurrentThread.ManagedThreadId;
				AnsMsg = $"S{d:00}：サーバーからクライアント{nums[0]}に応答";
				Ansbyte = Encoding.UTF8.GetBytes(AnsMsg);
				NS.Write(Ansbyte, 0, Ansbyte.Length);
				Array.Clear(Ansbyte, 0, Ansbyte.Length);
				Debug.WriteLine($"Server->Client{nums[0]} fin");

				if (int.TryParse(nums[0].ToString(),out int i))
				{
					if (i % 2 == 0)
					{
						//_ = Task.Factory.StartNew(() => heavymethod());
					}
				}

				this.Invoke((MethodInvoker)(() => textBox1.AppendText(AnsMsg + "\r\n")));

				//NS.Close();
			}

		}

		private void heavymethod()
		{
			Thread.Sleep(10000);
			Debug.WriteLine("処理終了");
		}
	}
}
