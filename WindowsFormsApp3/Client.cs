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
using System.Threading;

namespace WindowsFormsApp3
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        private readonly int _port = 39999;
        private ClientTCP CLtcp;

        private void Client_Load(object sender, EventArgs e)
        {
            CLtcp = new ClientTCP();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int n = (int)numericUpDown1.Value;
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"),_port);
            TcpClient[] CL = new TcpClient[n];
            //StringBuilder sb = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                CL[i] = new TcpClient();
				CL[i].ReceiveTimeout = 600000;
            }
            try
            {
				NetworkStream[] streams = new NetworkStream[n];
                Parallel.For(0, n, h =>
                {
                    Debug.WriteLine($"{h:00} -> Start!");
                    CL[h].Connect(EP.Address, EP.Port);
                    int d = Thread.CurrentThread.ManagedThreadId;

					streams[h] = CL[h].GetStream();

					Task[] tasks = new Task[2];
					for (int i = 0; i < 2; i++)
                    {

                        string msg = $"From Client{h:00} to Server Message Sending... {i + 1}times...";
                        byte[] sendByte = Encoding.UTF8.GetBytes(msg);
                        streams[h].Write(sendByte, 0, sendByte.Length);
                        this.Invoke((MethodInvoker)(() => textBox1.AppendText($"C{d:00}Send:{msg}\r\n")));

                        byte[] retByte = new byte[256];
                        _ = streams[h].Read(retByte, 0, 256);
                        string ret = Encoding.UTF8.GetString(retByte, 0, retByte.Length).TrimEnd('\0');
                        this.Invoke((MethodInvoker)(() => textBox1.AppendText($"C{d:00}Recieve:{ret}\r\n")));

						//Array.Clear(retByte, 0, retByte.Length);
						//textBox1.Refresh();
						tasks[i] =Task.Run(() =>
						{
							Thread.Sleep(1000);
						});
                    }
					Task.WaitAll(tasks);
					Debug.WriteLine($"{h:00} -> Fin?");

				});

			}
            catch (AggregateException ae)
            {
                var ex_l = ae.Flatten().InnerExceptions;

                foreach (var ex in ex_l)
                {
                    Debug.WriteLine(ex.GetType());
                }
            }
			finally
			{

			}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CLtcp.StartClient();
        }
    }
}
