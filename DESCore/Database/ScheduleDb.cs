using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class ScheduleDb
    {
        public static ResultModel Save(DESEntities db, Schedule schedule)
        {
            try
            {
                Schedule sch = db.Schedules.Where(r => r.SiteDataId == schedule.SiteDataId && 
                    r.ScheduleIndex == schedule.ScheduleIndex).FirstOrDefault();
                if (sch == null)
                {
                    schedule.DateUpdated = DateTime.Now;
                    db.Schedules.Add(schedule);
                }
                else
                {
                    sch.Start1Hour = schedule.Start1Hour;
                    sch.Start1Minute = schedule.Start1Minute;
                    sch.End1Hour = schedule.End1Hour;
                    sch.End1Minute = schedule.End1Minute;
                    sch.Day1 = schedule.Day1;
                    sch.Start2Hour = schedule.Start2Hour;
                    sch.Start2Minute = schedule.Start2Minute;
                    sch.End2Hour = schedule.End2Hour;
                    sch.End2Minute = schedule.End2Minute;
                    sch.Day2 = schedule.Day2;
                    sch.DateUpdated = DateTime.Now;
                    db.Entry(sch).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return new ResultModel { Status = true, };
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; }
        }

        public static string GetDescription(Schedule schedule)
        {
            return "ScheduleIndex:" + schedule.ScheduleIndex + ", " +
                "Start1:" + schedule.Start1Hour.ToString("D2") + ":" + schedule.Start1Minute.ToString("D2") + ", " +
                "End1:" + schedule.End1Hour.ToString("D2") + ":" + schedule.End1Minute.ToString("D2") + ", " +
                "Day1:" + schedule.Day1 + ", " +
                "Start2:" + schedule.Start2Hour.ToString("D2") + ":" + schedule.Start2Minute.ToString("D2") + ", " +
                "End2:" + schedule.End2Hour.ToString("D2") + ":" + schedule.End2Minute.ToString("D2") + ", " +
                "Day2:" + schedule.Day2;
        }
    }
}
