using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace DESCore.Sessions
{
    public class WebServer : TCPServer
    {
        public override void CallBack_AcceptDone(IAsyncResult ar)
        {
            try
            {
                Socket socket = ListenSocket.EndAccept(ar);
                if (socket != null)
                {
                    if (Log != null) Log("WebServer | Accepted | " + socket.RemoteEndPoint + ".");
                    WebSession webSession = new WebSession();
                    webSession.Log = ClientLog;
                    webSession.Start(socket);
                }

                ListenSocket.BeginAccept(new AsyncCallback(CallBack_AcceptDone), null);
                return;
            }
            catch { }
            ListenSocket = null;
        }
    }
}

