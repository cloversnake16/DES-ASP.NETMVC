using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Threading.Tasks;
using DESCore.General;
using DESCore.Models;
using DESCore.Helpers;
using DESCore.Database;
using DESCore.DesCommands;
using System.IO;

namespace DESCore.Sessions
{
    public class DesSession : TCPClient
    {
        private List<byte> _recvData = new List<byte>();
        private ManualResetEvent DoneEvent = new ManualResetEvent(false);
        public WebSession Fellow { get; set; }
        public bool IsBusy { get; set; }

        private List<byte> _recvBytes = new List<byte>();
        public DataModel DataModel { get; set; }
        public byte[] RecvBytes { get; set; }

        public override void Stop()
        {
            base.Stop();
            DesServer.RemoveDesSession(this);
            if (Device != null) DeviceDb.Disconnect(Device);
            Device = null;
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
                string s = ConfigurationManager.AppSettings["Response_Timeout"];
                DoneEvent.WaitOne(int.Parse(ConfigurationManager.AppSettings["Response_Timeout"]));
                return;
            }
            catch { }
            Stop();
        }

        public override void ParseData(byte[] data)
        {
            string device_status = "";
            try
            {
                if (Device == null)
                {
                    if (data.Length >= 21 && data[15] == 0 && data[20] == 0)
                    {
                        Device = ConvertHelper.GetDevice(data);
                        if (Device != null)
                        {
                            if (Log != null) Log("DesSession | Login | DeviceId: " + Device.DeviceId +
                                " | IpAddress: " + Device.IpAddress + " | PhoneNumber: " + Device.PhoneNumber +
                                " | Receive | " + ConvertHelper.GetString(data));

                            Device.IsConnect = true;                            
                            DesServer.AddDesSession(this);
                            DeviceDb.AddDevice(Device);
                            byte[] bytes = RetreiveCommand.RetreiveACMVersion();
                            if (Log != null) Log("DesSession | " + Device.DeviceId + " | Version | Send | " + ConvertHelper.GetString(bytes));
                            // added by me
                            device_status = DateTime.Now.ToString() + " : Device Created(IP:" + Device.IpAddress + ", Id:" + (Device.DeviceId != null ? Device.DeviceId : "Null") + ", ACMVersion:" + (Device.ACMVersion != null ? Device.ACMVersion : "Null") + ", Connection:" + Convert.ToString(Device.IsConnect) + ")" + Environment.NewLine;
                            device_status += "\t\t\t\t\t  Modem->Web app" + Encoding.UTF8.GetString(bytes) + Environment.NewLine;
                            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "device_log.txt", device_status);
                            //--------
                            SendData(bytes);
                        }
                    }
                    else
                    {
                        // added by me
                        device_status = DateTime.Now.ToString() + " : Protocol Dismatch(length="+Convert.ToString(data.Length)+" && data[15]="+Convert.ToString(data[15])+" && data[20]="+Convert.ToString(data[20])+"), Received Data:"+ Encoding.UTF8.GetString(data) + Environment.NewLine;
                        File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "device_log.txt", device_status);                        
                        //
                        Stop();
                    }
                }
                else
                {
                    _recvData.AddRange(ConvertHelper.Decoder(data));                    
                    while (true)
                    {
                        //
                        device_status = DateTime.Now.ToString() + " : (Data Length:" +(_recvData.Count()>0 ? Convert.ToString(_recvData.Count()) : "0")  + "), (Received Data:"+ Encoding.UTF8.GetString(_recvData.ToArray())+")"+Environment.NewLine;
                        File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "device_log.txt", device_status);
                        //
                        if (_recvData.Count < 18) return;

                        //
                        device_status = DateTime.Now.ToString() + " : " + Device.IpAddress+"(Received Data:";
                        device_status += "<DLE>:" + (_recvData.IndexOf(Constants.DLE)>=0 ? "OK" : "NO");
                        device_status += ", <ACK>:" + (_recvData.IndexOf(Constants.ACK) >= 0 ? "OK" : "NO");
                        device_status += ", <EOT>:" + (_recvData.IndexOf(Constants.EOT) >= 0 ? "OK" : "NO") +")" +Environment.NewLine;
                        File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "device_log.txt", device_status);
                        //

                        int index = _recvData.IndexOf(Constants.DLE);
                        if (index == -1)
                        {
                            _recvData.Clear();
                            return;
                        }
                        else if (index > 0) _recvData.RemoveRange(0, index);

                        if (_recvData.Count < 18) return;
                        if (_recvData[1] != Constants.ACK)
                        {
                            _recvData.RemoveRange(0, 1);
                            continue;
                        }

                        index = _recvData.IndexOf(Constants.EOT);
                        if (index == -1) return;

                        if (_recvData.Count < index + 3) return;

                        byte[] bytes = new byte[index + 3];
                        Array.Copy(_recvData.ToArray(), bytes, index + 3);

                        //
                        device_status = DateTime.Now.ToString() + " : " + Device.IpAddress;
                        device_status += ", Received Data:" + Encoding.UTF8.GetString(_recvData.ToArray());
                        device_status += ", (Prev Connection:" + (Convert.ToString(Device.IsConnect));
                        device_status +=", New Connection : "+(data[14] == Constants.VERSION ? "True" : "False") +" )" + Environment.NewLine;
                        File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "device_log.txt", device_status);
                        //

                        _recvData.RemoveRange(0, index + 3);
                        ParseCommand(bytes);

                        //
                        device_status =  "\t\t\t\t\t (Updated Connection:" + Convert.ToString(Device.IsConnect)+")" + Environment.NewLine;
                        File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "device_log.txt", device_status);
                        //

                        DataModel.DataType = DataType.DataReceived;
                        RecvBytes = bytes;
                        DoneEvent.Set();
                    }                    
                }
            }
            catch { DoneEvent.Set(); }
        }

        public void ParseCommand(byte[] data)
        {
            if (data[14] == Constants.VERSION)
            {
                Device.ACMVersion = RetreiveCommand.RetreiveACMVersion(data);
                if (Log != null) Log("DesSession | " + Device.DeviceId + " | Version: " + Device.ACMVersion + " | Receive | " + ConvertHelper.GetString(data));

                DeviceDb.Connect(Device);
            }
            else if (Fellow != null)
            {
                if (Log != null) Log("DesSession | " + Fellow.Device.DeviceId + " | DataReceived | Receive | " + ConvertHelper.GetString(data));
                byte[] bytes = ConvertHelper.GetBytes(DataType.DataReceived, data);
                Fellow.SendData(bytes);
            }
            DeviceDb.AddInBound(Device.DeviceId, data.Length);
        }

        public override void MainThreadProc()
        {
            IsBusy = false;
            BackgroundTaskProc();
        }

        private async void BackgroundTaskProc()
        {
            try
            {
                if (Device != null && !string.IsNullOrEmpty(Device.ACMVersion) && Socket != null && Fellow == null)
                {
                    Json siteModel = SiteDb.GetSite(Device.DeviceId);
                    while (Device != null && !string.IsNullOrEmpty(Device.ACMVersion) && Socket != null && Fellow == null)
                    {
                        BackTask backTask = BackTaskDb.GetBackTask(Device.DeviceId);
                        if (backTask == null) break;
                           
                        IsBusy = true;

                        if(backTask.WorkType == SiteDataType.LastProgrammed.ToString())
                        {
                            SiteParaModel siteParaModel = BackTaskDb.GetSiteParaModel(backTask);
                            ResultModel result = SiteDataDb.LoadParameter(siteParaModel, siteModel);
                            if (!result.Status) break;

                            siteParaModel = (SiteParaModel)result.Result;
                            if (siteParaModel.SiteParaType != SiteParaType.Channel || siteParaModel.Channel != null)
                            {
                                result = BackTaskHelper.SaveParameter(this, siteParaModel, siteModel);
                                if (!result.Status)
                                {
                                    backTask.WorkStatus = WorkStatusType.Failed.ToString();
                                    backTask.Description = (string)result.Result;
                                    BackTaskDb.UpdateBackTask(backTask);
                                    break;
                                }

                                result = SiteDataDb.GetSiteDataId(siteModel.Site, SiteDataType.LastProgrammed);
                                if (!result.Status) break;

                                siteParaModel.SiteData.Id = (int)result.Result;
                                result = SiteDataDb.SaveParameter(siteParaModel, siteModel);
                                if (!result.Status) break;
                            }

                            backTask = BackTaskDb.NextBackTask(backTask);
                            BackTaskDb.UpdateBackTask(backTask);
                        }
                        else if (backTask.WorkType == SiteDataType.ProgramStaff.ToString())
                        {
                            SiteParaModel siteParaModel = BackTaskDb.GetSiteParaModel(backTask);
                            if (siteParaModel == null)
                            {
                                backTask.WorkStatus = WorkStatusType.Failed.ToString();
                                backTask.Description = "Couldn't find staff group.";
                                BackTaskDb.UpdateBackTask(backTask);
                                break;
                            }

                            ResultModel result = BackTaskHelper.SaveParameter(this, siteParaModel, siteModel);
                            if (!result.Status)
                            {
                                backTask.WorkStatus = WorkStatusType.Failed.ToString();
                                backTask.Description = (string)result.Result;
                                BackTaskDb.UpdateBackTask(backTask);
                                break;
                            }

                            result = SiteDataDb.GetSiteDataId(siteModel.Site, SiteDataType.LastProgrammed);
                            if (!result.Status) break;

                            siteParaModel.SiteData.Id = (int)result.Result;
                            result = SiteDataDb.SaveParameter(siteParaModel, siteModel);
                            if (!result.Status) break;

                            backTask = BackTaskDb.NextBackTask(backTask);
                            BackTaskDb.UpdateBackTask(backTask);
                        }
                    }
                }
            }
            catch { }
            
            IsBusy =  false;
            await Task.Delay(1000);
            if (Socket != null) BackgroundTaskProc();
        }
    }
}
