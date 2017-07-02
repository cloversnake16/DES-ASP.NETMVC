using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class SystemOptionDb
    {
        public static ResultModel Save(DESEntities db, SystemOption systemOption)
        {
            try
            {
                SystemOption so = db.SystemOptions.Where(r => r.SiteDataId == systemOption.SiteDataId).FirstOrDefault();
                if (so == null)
                {
                    systemOption.DateUpdated = DateTime.Now; 
                    db.SystemOptions.Add(systemOption);
                }
                else
                {
                    so.Option1 = systemOption.Option1;
                    so.Option2 = systemOption.Option2;
                    so.TradeSchedule = systemOption.TradeSchedule;
                    so.RingTimeout = systemOption.RingTimeout;
                    so.AudioTimeout = systemOption.AudioTimeout;
                    so.WardenChannel = systemOption.WardenChannel;
                    so.CustomerNo = systemOption.CustomerNo;
                    so.SiteNo = systemOption.SiteNo;
                    so.DateUpdated = DateTime.Now;
                    db.Entry(so).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return new ResultModel { Status = true, };
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; }
        }

        public static string GetDescription(SystemOption systemOption)
        {
            return "Option1:" + systemOption.Option1 + ", " +
                "Option2:" + systemOption.Option2 + ", " +
                "TradeSchedule:" + systemOption.TradeSchedule + ", " +
                "RingTimeout:" + systemOption.RingTimeout + ", " +
                "AudioTimeout:" + systemOption.AudioTimeout + ", " +
                "WardenChannel:" + systemOption.WardenChannel + ", " +
                "CustomerNo:" + systemOption.CustomerNo + ", " +
                "SiteNo:" + systemOption.SiteNo;
        }
    }
}
