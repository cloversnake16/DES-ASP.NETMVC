using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using DESCore.General;
using DESCore.Helpers;
using DESCore.Models;
using DESCore.Database;

namespace DESCore.Sessions
{
    public class TcpSession : TCPClient
    {
        private string _recvData = "";
        private ManualResetEvent DoneEvent = new ManualResetEvent(false);

        private List<byte> _recvBytes = new List<byte>();
        public DataModel DataModel { get; set; }
        public byte[] RecvBytes { get; set; }

        public bool Connect()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip;
                int port = 5000;

                if (IPAddress.TryParse(ConfigurationManager.AppSettings["Server"], out ip)) socket.Connect(ip, port);
                else socket.Connect(ConfigurationManager.AppSettings["Server"], port);

                Start(socket);
                
                return true;
            }
            catch { }
            return false;
        }

        public override void Stop()
        {
            base.Stop();
            DoneEvent.Set();
        }

        public void Send(byte[] sendBuffer)
        {
            try
            {
                _recvBytes.Clear();
                RecvBytes = null;
                DataModel = new DataModel { DataType = DataType.NoReply };
                DoneEvent.Reset();
                SendData(sendBuffer);
                DoneEvent.WaitOne(int.Parse(ConfigurationManager.AppSettings["Response_Timeout"]));
                return;
            }
            catch { }
            Stop();
        }

        public override void ParseData(byte[] data)
        {
            try
            {
                _recvData += Encoding.UTF8.GetString(data);
                while (_recvData.Length > 0)
                {
                    int index = _recvData.IndexOf("{");
                    if (index == -1) break;
                    _recvData = _recvData.Substring(index);

                    index = _recvData.IndexOf("}");
                    if (index == -1) return;

                    string strData = _recvData.Substring(1, index - 1);
                    _recvData = _recvData.Substring(index + 1);

                    byte[] bytes = Convert.FromBase64String(strData);
                    DataModel dataModel = XmlHelper.GetObject<DataModel>(bytes);
                    if (dataModel != null)
                    {
                        if (dataModel.DataType == DataType.DataReceived)
                        {
                            bytes = Convert.FromBase64String(dataModel.Data);
                            bytes = XmlHelper.GetObject<byte[]>(bytes);

                            _recvBytes.AddRange(bytes);
                            index = _recvBytes.IndexOf(Constants.EOT);
                            if (index > 0 && _recvBytes.Count >= index + 3)
                            {
                                _recvBytes.RemoveRange(index + 3, _recvBytes.Count - (index + 3));
                                RecvBytes = _recvBytes.ToArray();
                                DataModel = dataModel;
                                DoneEvent.Set();
                            }
                        }
                        else
                        {
                            DataModel = dataModel;
                            DoneEvent.Set();
                        }
                    }
                }
            }
            catch { DoneEvent.Set(); }            
        }
    }
}
