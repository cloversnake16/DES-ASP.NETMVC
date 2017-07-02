using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;
using DESCore.Helpers;
using DESCore.Database;

namespace DESCore.Sessions
{
    public class WebSession : TCPClient
    {
        private string _recvData = "";
        public DesSession Fellow { get; set; }

        public override void Stop()
        {
            base.Stop();
            if (Fellow != null) 
            {
                Fellow.Fellow = null;
                Fellow = null;
            }
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
                        if (dataModel.DataType == DataType.Login)
                        {
                            bytes = Convert.FromBase64String(dataModel.Data);
                            string deviceId = XmlHelper.GetObject<string>(bytes);
                            Device = new Device { DeviceId = deviceId };
                            DesSession desSession = DesServer.FindDesSession(deviceId);
                            if (desSession == null)
                            {
                                bytes = ConvertHelper.GetBytes(DataType.NoExist);
                            }
                            else if (desSession.Fellow == null && !desSession.IsBusy)
                            {
                                Fellow = desSession;
                                desSession.Fellow = this;
                                bytes = ConvertHelper.GetBytes(DataType.Login);
                            }
                            else bytes = ConvertHelper.GetBytes(DataType.Busy);
                            SendData(bytes);
                        }
                        else if (dataModel.DataType == DataType.DataReceived && Fellow != null)
                        {
                            bytes = Convert.FromBase64String(dataModel.Data);
                            bytes = XmlHelper.GetObject<byte[]>(bytes);
                            if (Log != null) Log("DesSession | " + Device.DeviceId + " | Send | " + ConvertHelper.GetString(bytes));
                            Fellow.SendData(ConvertHelper.Encoder(bytes));
                            DeviceDb.AddOutBound(Fellow.Device.DeviceId, bytes.Length);
                        }
                    }
                }
            }
            catch { }
            _recvData = "";
        }
    }
}
