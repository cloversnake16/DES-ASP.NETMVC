using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class HomeDb
    {
        public static List<Notification> GetNotifications(int userId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.Notifications.Where(r => r.UserId == userId).OrderByDescending(r => r.Id).ToList();
                }
            }
            catch { }
            return null;
        }

        public static ResultModel RemoveNotification(int id)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var notification = db.Notifications.Where(r => r.Id == id).FirstOrDefault();
                    if (notification != null)
                    {
                        db.Notifications.Remove(notification);
                        db.SaveChanges();
                    }

                    return new ResultModel { Status = true, Result = "Deleted staff successfully." };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; };
        }
    }
}
