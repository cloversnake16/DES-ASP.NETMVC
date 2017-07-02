using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using DESCore.Helpers;
using DESCore.Models;
using System.IO;
using DESCore.Database;

namespace DESCore.Sessions
{
    public class TCPClient : IDisposable
    {
        private byte[] _recvBuffer = new byte[1024];
        private object _lockSend = new object();
        public Socket Socket { get; set; }
        public Device Device { get; set; }

        public delegate void DelegateResponseDone(object sender, DataModel dataModel);
        public delegate void DelegateLog(string msg);
        public DelegateResponseDone ResponseDone { get; set; }
        public DelegateLog Log { get; set; }
        public virtual void ParseData(byte[] data) { }
        public virtual void MainThreadProc() { }

        public void Start(Socket socket)
        {
            try
            {
                Socket = socket;

                Socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length,
                   SocketFlags.None, new AsyncCallback(CallBack_ReceiveDone), Socket);

                new Task(MainThreadProc).Start();
            }
            catch { }
        }

        public virtual void Stop()
        {
            try
            {
                if (Socket != null) Socket.Close();
            }
            catch { }
            Socket = null;
        }

        private void CallBack_ReceiveDone(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int intSize = socket.EndReceive(ar);
                // added by me
                string device_status = DateTime.Now.ToString() + " : Reveived Data:" + (intSize>0 ? Convert.ToString(intSize) : "No Data")+", Device:"+(Device!=null ? Device.DeviceId : "Null") + Environment.NewLine;
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "device_log.txt", device_status);
                //
                if (intSize > 0)
                {                    
                    byte[] data = new byte[intSize];
                    Array.Copy(_recvBuffer, data, intSize);
                    ParseData(data);

                    socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length,
                       SocketFlags.None, new AsyncCallback(CallBack_ReceiveDone), socket);
                    return;
                }
                else
                {
                    //DeviceDb.Disconnect(Device);
                    if (Device != null)
                    {
                        DeviceDb.Disconnect(Device);
                    }
                    else
                    {
                        List<Device> devices = DeviceDb.GetDevices();
                        for (int i=0; i<devices.Count(); ++i)
                        {
                            DeviceDb.Disconnect(devices.ElementAt(i));
                        }
                    }
                }
            }
            
            catch { }
            Stop();
        }

        public void SendData(byte[] sendBytes)
        {
            try
            {
                if (sendBytes == null || sendBytes.Length == 0) return;

                lock(_lockSend)
                {                    
                    int offset = 0;
                    while (offset < sendBytes.Length)
                    {
                        offset += Socket.Send(sendBytes, offset, sendBytes.Length - offset, SocketFlags.None);
                    }
                }
                // added by me
                string device_status = DateTime.Now.ToString() + " : Web app->Modem" + Encoding.UTF8.GetString(sendBytes) + Environment.NewLine;
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "device_log.txt", device_status);
                //
                return;
            }
            catch { }
            Stop();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
