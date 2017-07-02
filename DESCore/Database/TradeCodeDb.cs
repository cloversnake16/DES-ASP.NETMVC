using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class TradeCodeDb
    {
        public static ResultModel Save(DESEntities db, TradeCode tradeCode)
        {
            try
            {
                TradeCode tc = db.TradeCodes.Where(r => r.SiteDataId == tradeCode.SiteDataId &&
                    r.TradeCodeIndex == tradeCode.TradeCodeIndex).FirstOrDefault();
                if (tc == null)
                {
                    tradeCode.DateUpdated = DateTime.Now;
                    db.TradeCodes.Add(tradeCode);
                }
                else
                {
                    tc.PassNumber = tradeCode.PassNumber;
                    tc.ScheduleIndex = tradeCode.ScheduleIndex;
                    tc.DateUpdated = DateTime.Now;
                    db.Entry(tc).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return new ResultModel { Status = true, };
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; }
        }

        public static string GetDescription(TradeCode tradeCode)
        {
            return "TradeCodeIndex:" + tradeCode.TradeCodeIndex + ", " +
                "PassNumber:" + tradeCode.PassNumber + ", " +
                "ScheduleIndex:" + tradeCode.ScheduleIndex;
        }
    }
}
