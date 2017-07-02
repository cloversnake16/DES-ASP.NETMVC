using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class DoorDb
    {
        public static ResultModel Save(DESEntities db, Door door)
        {
            try
            {
                Door dr = db.Doors.Where(r => r.SiteDataId == door.SiteDataId &&
                    r.DoorIndex == door.DoorIndex).FirstOrDefault();
                if (dr == null)
                {
                    door.DateUpdated = DateTime.Now;
                    db.Doors.Add(door);
                }
                else
                {
                    dr.LockTimeout = door.LockTimeout;
                    dr.ScheduleIndex = door.ScheduleIndex;
                    dr.DateUpdated = DateTime.Now;
                    db.Entry(dr).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return new ResultModel { Status = true, };
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; }
        }

        public static string GetDescription(Door door)
        {
            return "DoorIndex:" + door.DoorIndex + ", " +
                "LockTimeout:" + door.LockTimeout + ", " +
                "ScheduleIndex:" + door.ScheduleIndex;
        }
    }
}
