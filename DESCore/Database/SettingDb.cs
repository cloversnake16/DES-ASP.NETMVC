using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DESCore.Models;

namespace DESCore.Database
{
    public class SettingDb
    {
        public static Setting GetSetting()
        {
            try
            {
                using (var db = new DESEntities())
                {
                    Setting settings = db.Settings.FirstOrDefault();
                    if (settings == null)
                    {
                        settings = new Setting
                        {
                            CheckPeriod = int.Parse(ConfigurationManager.AppSettings["CheckPeriod"]),
                            MaxDataSize = int.Parse(ConfigurationManager.AppSettings["MaxDataSize"]),
                            MaxEventlogSize = int.Parse(ConfigurationManager.AppSettings["MaxEventlogSize"]),
                            DateReset = new DateTime(2000, 1, 1),
                            ReservedChannel = 0,
                            DateUpdated = DateTime.Now,
                        };
                        settings = db.Settings.Add(settings);
                        db.SaveChanges();
                    }
                    return settings;
                }
            }
            catch { }
            return null;
        }

        public static ResultModel SetAlertSetting(Setting model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    Setting settings = GetSetting();
                    settings.CheckPeriod = model.CheckPeriod;
                    settings.MaxDataSize = model.MaxDataSize;
                    settings.MaxEventlogSize = model.MaxEventlogSize;
                    settings.DateReset = model.DateReset;
                    settings.DateUpdated = DateTime.Now;
                    db.Entry(settings).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new ResultModel { Status = true, Result = "Updated successfully." };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public static int[] GetReservedChannelIndics()
        {
            try
            {
                List<int> reservedChannelIndics = new List<int>();

                int reservedChannel = GetSetting().ReservedChannel;
                for (var i = 0; i < 16; i++)
                {
                    if ((reservedChannel & (1 << i)) != 0)
                    {
                        for (var j = 0; j < 16; j++) reservedChannelIndics.Add(i * 16 + j);
                    }
                }
                return reservedChannelIndics.ToArray();
            }
            catch (Exception) { }
            return null;
        }

        public static ResultModel SaveReservedChannels(int reservedChannels)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    Setting settings = GetSetting();
                    settings.ReservedChannel = reservedChannels;
                    settings.DateUpdated = DateTime.Now;
                    db.Entry(settings).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new ResultModel { Status = true, Result = "Updated successfully." };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public static Description[] GetDoorDescriptions()
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.Descriptions.Where(r => r.Type == "Door").ToArray();
                }
            }
            catch { }
            return null;
        }

        public static ResultModel SaveDoorDescription(Description model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    if (model.Contents == null) model.Contents = "";
                    var description = db.Descriptions.Where(r => r.Type == "Door" && r.Index == model.Index).FirstOrDefault();
                    if(description == null)
                    {
                        description = new Description
                        {                            
                            Type = "Door",
                            Index = model.Index,
                            Contents = model.Contents,
                            DateUpdated = DateTime.Now,
                        };
                        db.Descriptions.Add(description);
                    }
                    else
                    {
                        description.Contents = model.Contents;
                        description.DateUpdated = DateTime.Now;
                        db.Entry(description).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                    return new ResultModel { Status = true, Result = "Saved successfully." };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }
    }
}
