using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using DESCore.Models;
using DESCore.Database;
using DESCore.Helpers;
using System.IO;

namespace DESCore.Sessions
{
    public class DesServer : TCPServer
    {
        private static Dictionary<string, DesSession> _dicDesSession = new Dictionary<string, DesSession>();

        public override void CallBack_AcceptDone(IAsyncResult ar)
        {
            try
            {                
                Socket socket = ListenSocket.EndAccept(ar);
                // added by me
                string device_status = DateTime.Now.ToString() + " : " +  (socket!=null ? socket.RemoteEndPoint.ToString() : "null")+ Environment.NewLine;
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "server_socket.txt", device_status);
                //--------
                if (socket != null)
                {
                    if (Log != null) Log("DesServer | Accepted | " + socket.RemoteEndPoint + ".");
                    DesSession desSession = new DesSession();
                    desSession.Log = ClientLog;
                    desSession.Start(socket);
                }                
                ListenSocket.BeginAccept(new AsyncCallback(CallBack_AcceptDone), null);
                return;
            }
            catch { }
            ListenSocket = null;
        }

        public static void AddDesSession(DesSession desSession)
        {
            try
            {
                lock (_dicDesSession)
                {
                    _dicDesSession[desSession.Device.DeviceId] = desSession;
                }
            }
            catch { }
        }

        public static void RemoveDesSession(DesSession desSession)
        {
            try
            {
                lock (_dicDesSession)
                {
                    if (_dicDesSession.ContainsKey(desSession.Device.DeviceId)) _dicDesSession.Remove(desSession.Device.DeviceId);
                }
            }
            catch { }
        }

        public static DesSession FindDesSession(string deviceId)
        {
            try
            {
                lock (_dicDesSession)
                {
                    if (_dicDesSession.ContainsKey(deviceId)) return _dicDesSession[deviceId];
                }
            }
            catch { }
            return null;
        }

        public override void MainThreadProc()
        {
            try
            {
                AlertProc();
                RemoteSiteEventLogProc();
            }
            catch { }
        }

        private async void AlertProc()
        {
            try
            {
                ListAlertModel listAlertModel = DeviceDb.GetListAlertModel();
                if (listAlertModel != null)
                {
                    foreach (var alertModel in listAlertModel.AlertModels)
                    {
                        await EmailHelper.SendEmail(alertModel.Email, "Site Status Report", alertModel.AlertMsg);
                    }
                    await Task.Delay(listAlertModel.CheckPeroid * 60 * 1000);
                }
            }
            catch { }

            await Task.Delay(1000);
            if (ListenSocket != null) AlertProc();
        }

        private async void RemoteSiteEventLogProc()
        {
            await Task.Delay(1000);
            if (ListenSocket != null) RemoteSiteEventLogProc();
        }
    }
}

