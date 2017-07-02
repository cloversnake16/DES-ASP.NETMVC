using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class StaffAccessDb
    {
        public static ResultModel Save(DESEntities db, StaffAccess staffAccess)
        {
            try
            {
                StaffAccess sa = db.StaffAccesses.Where(r => r.SiteDataId == staffAccess.SiteDataId &&
                    r.StaffAccessIndex == staffAccess.StaffAccessIndex).FirstOrDefault();
                if (sa == null)
                {
                    staffAccess.DateUpdated = DateTime.Now;
                    db.StaffAccesses.Add(staffAccess);
                }
                else
                {
                    sa.AccessLevel = staffAccess.AccessLevel;
                    sa.PassNumber = staffAccess.PassNumber;
                    sa.DateUpdated = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return new ResultModel { Status = true, };
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; }
        }

        public static string GetDescription(StaffAccess staffAccess)
        {
            return "StaffAccessIndex:" + staffAccess.StaffAccessIndex + ", " +
                "AccessLevel:" + staffAccess.AccessLevel + ", " +
                "PassNumber:" + staffAccess.PassNumber;
        }
    }
}
