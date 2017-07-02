using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;
using DESCore.General;
using DESCore.Helpers;
using System.Globalization;

namespace DESCore.Database
{
    public class DeviceDb
    {
        public static List<Device> GetDevices()
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.Devices.ToList();
                }
            }
            catch { }
            return null;
        }

        public static Device GetDevice(string deviceId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.Devices.Where(r => r.DeviceId == deviceId).FirstOrDefault();
                }
            }
            catch { }
            return null;
        }

        public static void AddDevice(Device model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var device = db.Devices.Where(r => r.DeviceId == model.DeviceId).FirstOrDefault();
                    if (device == null)
                    {
                        device = new Device
                        {
                            DeviceId = model.DeviceId,
                            PhoneNumber = model.PhoneNumber,
                            IpAddress = model.IpAddress,
                            ACMVersion = "",
                            InBound = 0,
                            OutBound = 0,
                            IsConnect = true,
                            DateUpdated = DateTime.Now,
                        };
                        db.Devices.Add(device);
                    }
                    else
                    {
                        device.PhoneNumber = model.PhoneNumber;
                        device.IpAddress = model.IpAddress;
                        device.ACMVersion = "";
                        device.IsConnect = true;
                        device.DateUpdated = DateTime.Now;
                        db.Entry(device).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            catch { }
        }

        public static void Connect(Device model)
        {
            try
            {
                if (model == null) return;

                using (var db = new DESEntities())
                {
                    var device = db.Devices.Where(r => r.DeviceId == model.DeviceId).FirstOrDefault();
                    if (device == null)
                    {
                        device = new Device
                        {
                            DeviceId = model.DeviceId,
                            PhoneNumber = model.PhoneNumber,
                            IpAddress = model.IpAddress,
                            ACMVersion = model.ACMVersion,
                            InBound = 0,
                            OutBound = 0,
                            IsConnect = true,
                            DateUpdated = DateTime.Now,
                        };
                        db.Devices.Add(device);
                    }
                    else
                    {
                        device.ACMVersion = model.ACMVersion;
                        device.IsConnect = true;
                        device.DateUpdated = DateTime.Now;
                        db.Entry(device).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            catch { }
        }

        public static void Disconnect(Device model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var device = db.Devices.Where(r => r.DeviceId == model.DeviceId).FirstOrDefault();
                    if (device != null)
                    {
                        device.ACMVersion = "";
                        device.IsConnect = false;
                        device.DateUpdated = DateTime.Now;
                        db.Entry(device).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch { }
        }

        public static void AddInBound(string deviceId, long inbound)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var device = db.Devices.Where(r => r.DeviceId == deviceId).FirstOrDefault();
                    if (device != null)
                    {
                        device.InBound += inbound;
                        device.DateUpdated = DateTime.Now;
                        db.Entry(device).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch { }
        }

        public static void AddOutBound(string deviceId, long outbound)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var device = db.Devices.Where(r => r.DeviceId == deviceId).FirstOrDefault();
                    if (device != null)
                    {
                        device.OutBound += outbound;
                        device.DateUpdated = DateTime.Now;
                        db.Entry(device).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch { }
        }

        public static bool ResetInOutBound(DateTime dateReset)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    DateTime now = DateTime.Today;
                    if (now.Month == dateReset.Month && now.Day == dateReset.Day) 
                    {
                        var devices = db.Devices.Where(r => r.InBound > 0 || r.OutBound > 0) .ToArray();
                        if(devices != null)
                        {
                            foreach(var device in devices)
                            {
                                device.InBound = 0;
                                device.OutBound = 0;
                                device.DateUpdated = DateTime.Now;
                                db.Entry(device).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                }
                return true;
            }
            catch { }
            return false;
        }

        public static ListAlertModel GetListAlertModel()
        {
            try
            {
                Setting setting = SettingDb.GetSetting();
                ListAlertModel listAlertModel = new ListAlertModel{AlertModels = new List<AlertModel>(), CheckPeroid = setting.CheckPeriod, };

                DeviceDb.ResetInOutBound(setting.DateReset);
                                       
                using (var db = new DESEntities())
                {
                    string adminBody = "";
                    var users = db.Users.ToArray();
                    foreach (var user in users)
                    {
                        var sites = user.Sites;
                        if (sites.Count == 0) continue;

                        string userBody = "";
                        foreach (var site in sites)
                        {
                            if (string.IsNullOrEmpty(site.DeviceId)) continue;

                            var device = db.Devices.Where(r => r.DeviceId == site.DeviceId).FirstOrDefault();
                            if (device == null) continue;

                            bool alert = false;
                            string siteBody = "";

                            if (string.IsNullOrEmpty(device.ACMVersion))
                            {
                                siteBody += "<p>Modem Status: " + (device.IsConnect?("Connect"):("Disconnect")) + "</p>";
                                siteBody += "<p>System Status: " + (!string.IsNullOrEmpty(device.ACMVersion) ? ("Connect") : ("Disconnect")) + "</p>";
                                alert = true;
                            }

                            long dataBound = (device.InBound + device.OutBound) / 1024 / 1024;
                            if (dataBound > 0.9 * setting.MaxDataSize)
                            {
                                siteBody += "<p>InBound: " + GetSize((ulong)device.InBound) + ", ";
                                siteBody += "OutBound: " + GetSize((ulong)device.OutBound) + ", ";
                                siteBody += "DataBound: " + dataBound + "MB (" + (dataBound * 100 / setting.MaxDataSize) + "%)" + "</p>";
                                alert = true;
                            }

                            int eventLogCount = db.EventLogs.Where(r => r.SiteId == site.Id).Count();
                            if (eventLogCount > 0.9 * setting.MaxEventlogSize)
                            {
                                siteBody += "<p>EventLogCount: " + eventLogCount + "(" + (eventLogCount * 100 / setting.MaxEventlogSize) + "%)" + "</p>";
                                new EventLogDb().Delete(site.Id, Convert.ToInt32(0.8 * setting.MaxEventlogSize));
                                alert = true;
                            }

                            if(alert)
                            {
                                userBody += "<p>SiteName: " + site.SiteName + ", DeviceId: " + site.DeviceId + "</p>";
                                userBody += siteBody + "<br/>";
                            }
                        }

                        if (!string.IsNullOrEmpty(userBody))
                        {
                            userBody = "<p><b>UserName: " + AccountDb.UserFullName(user) + "</b></p>" + userBody + "<br/>";
                            if (user.UserType.TypeName != Constants.SysAdmin)
                            {
                                listAlertModel.AlertModels.Add(new AlertModel
                                {
                                    Email = user.Email,
                                    AlertMsg = userBody,
                                });
                            }
                            adminBody += userBody;
                        }
                    }

                    if(!string.IsNullOrEmpty(adminBody))
                    {
                        var adminUsers = db.Users.Where(r => r.UserType.TypeName == Constants.SysAdmin).ToArray();
                        foreach(var adminUser in adminUsers)
                        {
                            listAlertModel.AlertModels.Add(new AlertModel
                            {
                                Email = adminUser.Email,
                                AlertMsg = adminBody,
                            });
                        }
                    }

                    if (listAlertModel.AlertModels.Count > 0) return listAlertModel;
                }
            }
            catch { }
            return null;
        }

        public static double GetVersion(string ACMVersion)
        {            
            double d = 0;
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            if (!string.IsNullOrEmpty(ACMVersion)) double.TryParse(ACMVersion.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0], style, culture, out d);
            return d;
        }

        public static string Connected(string ACMVersion)
        {
            return string.IsNullOrEmpty(ACMVersion) ? ("Disconnected") : ("Connected");
        }

        public static string GetSize(UInt64 size)
        {
            if (size < 1024) return string.Format("{0} B", size);

            double dSize = Convert.ToDouble(size) / 1024.0;
            if (dSize < 1024.0) return dSize.ToString("N") + " KB";

            dSize = Convert.ToDouble(size) / 1024.0;
            if (dSize < 1024.0) return dSize.ToString("N") + " MB";

            dSize = Convert.ToDouble(size) / 1024.0;
            return dSize.ToString("N") + " GB";
        }
    }
}
