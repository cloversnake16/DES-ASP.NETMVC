using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using DESCore.Models;
using DESCore.Helpers;

namespace DESCore.Sessions
{
    public class TCPServer
    {
        public delegate void DelegateResponseDone(object sender, DataModel dataModel);
        public delegate void DelegateLog(string msg);
        public Socket ListenSocket { get; set; }
        public DelegateResponseDone ResponseDone { get; set; }
        public DelegateLog Log { get; set; }

        public virtual void MainThreadProc() { }
        public virtual void CallBack_AcceptDone(IAsyncResult ar) { }

        public void Listen(int port)
        {
            try
            {
                Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                listenSocket.Listen(5000);
                listenSocket.BeginAccept(new AsyncCallback(CallBack_AcceptDone), null);

                ListenSocket = listenSocket;
                if (Log != null) Log("Listening(Port:" + port + ")...");

                new Task(MainThreadProc).Start();
                return;
            }
            catch { }
            ListenSocket = null;
        }

        public void Stop()
        {
            try
            {
                if (ListenSocket != null) ListenSocket.Close();
            }
            catch { }
            ListenSocket = null;
        }

        public void ClientLog(string msg)
        {
            if (Log != null) Log(msg);
        }
    }
}