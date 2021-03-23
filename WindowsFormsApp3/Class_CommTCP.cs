using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;

    
namespace WindowsFormsApp3
{
    delegate void SendDelegate(Socket _soc);

    class Class_CommTCP
    {
        private ManualResetEvent SocketEvent = new ManualResetEvent(false);
        private readonly IPEndPoint IPE;
        private Socket soc;
        private Form FM;
        private TextBox tb;
        private Encoding ENC;

        public Class_CommTCP(IPEndPoint iep,Encoding enc,Form _FM, TextBox _tb)
        {
            FM = _FM;
            tb = _tb;
            IPE = iep;
            ENC = enc;
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soc.Bind(IPE);
            soc.Listen(10);
            var tM = new Thread(new ThreadStart(Round));
            tM.Start();
        }

        private void Round()
        {
            while (true)
            {
                SocketEvent.Reset();
                soc.BeginAccept(new AsyncCallback(OnconnectRequest), soc);
                SocketEvent.WaitOne();
            }
        }

        private void OnconnectRequest(IAsyncResult ar)
        {
            SocketEvent.Set();
            Socket listner = (Socket)ar.AsyncState;
            Socket handler = listner.EndAccept(ar);
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        void ReadCallback(IAsyncResult ar)
        {
            Console.WriteLine("ReadCallback ThreadID:" + Thread.CurrentThread.ManagedThreadId);
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int ReadSize = handler.EndReceive(ar);
            if (ReadSize < 1)
            {
                Console.WriteLine(handler.RemoteEndPoint.ToString() + " disconnected");
                return;
            }
            byte[] bb = new byte[ReadSize];
            Array.Copy(state.buffer, bb, ReadSize);
            string msg = System.Text.Encoding.UTF8.GetString(bb);
            FM.Invoke((MethodInvoker)(() => tb.AppendText(msg + "\r\n")));
            Console.WriteLine(msg);
            handler.BeginSend(bb, 0, bb.Length, 0, new AsyncCallback(WriteCallback), state);
        }

        void  WriteCallback(IAsyncResult ar)
        {
            Console.WriteLine("WriteCallback ThreadID:" + Thread.CurrentThread.ManagedThreadId);
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            handler.EndSend(ar);

            Console.WriteLine("送信完了");
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        void disConnect()
        {
            Console.WriteLine("disConnect ThreadID:" + Thread.CurrentThread.ManagedThreadId);
            soc.Close();
        }

        public void send(Socket handler, string msg)
        {
            byte[] sd = ENC.GetBytes(msg);

        }

        public class StateObject
        {
            // Client socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 256;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }
    }
}
