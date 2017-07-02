using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;
using DESCore.Helpers;

namespace DESCore.Database
{
    public class NotificationDb
    {
        public static void AddNotification(Notification notification, DESEntities db)
        {
            try
            {
                if (notification == null) return;

                if (db == null)
                {
                    using (var entity = new DESEntities())
                    {
                        AddNotification(notification, entity);
                    }
                }
                else
                {
                    notification.DateUpdated = DateTime.Now;
                    db.Notifications.Add(notification);
                    db.SaveChanges();
                }
            }
            catch { }
        }

        public static DataModel Notification(NotificationModel notificationModel)
        {
            try
            {
                if (notificationModel.NotificationType == NotificationType.BackTasks)
                {
                    return BackTasks(notificationModel);
                }
                else if (notificationModel.NotificationType == NotificationType.RemoveNotification)
                {
                    return RemoveNotification(notificationModel);
                }
                else if (notificationModel.NotificationType == NotificationType.RetryBackTask)
                {
                    return RetryBackTask(notificationModel);
                }
                else if (notificationModel.NotificationType == NotificationType.RemoveBackTask)
                {
                    return RemoveBackTask(notificationModel);
                }
                using (var db = new DESEntities())
                {
                    int userId = AccountDb.ValidUser(notificationModel.UserName, notificationModel.Password, db);
                    if (userId > 0)
                    {
                        Notification[] notifications = db.Notifications.Where(r => r.UserId == userId).OrderByDescending(r => r.Id).Take(20).ToArray();
                        return ConvertHelper.GetDataModel(DataType.Notifications, notifications);
                    }
                    else return ConvertHelper.GetDataModel(DataType.Failure, "Invalid username or password");
                }
            }
            catch (Exception ex) { return ConvertHelper.GetDataModel(DataType.Failure, ex.Message); }
        }

        public static DataModel BackTasks(NotificationModel notificationModel)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    int userId = AccountDb.ValidUser(notificationModel.UserName, notificationModel.Password, db);
                    if (userId > 0)
                    {
                        BackTask[] backTasks = db.BackTasks.Where(r => r.UserId == userId).OrderByDescending(r => r.Id).Take(20).ToArray();
                        return ConvertHelper.GetDataModel(DataType.BackTasks, ToNotifications(backTasks));
                    }
                    else return ConvertHelper.GetDataModel(DataType.Failure, "Invalid username or password");
                }
            }
            catch (Exception ex) { return ConvertHelper.GetDataModel(DataType.Failure, ex.Message); }
        }

        public static DataModel RemoveNotification(NotificationModel notificationModel)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    int userId = AccountDb.ValidUser(notificationModel.UserName, notificationModel.Password, db);
                    if (userId > 0)
                    {
                        var notification = db.Notifications.Where(r => r.Id == notificationModel.Id).FirstOrDefault();
                        if(notification != null)
                        {
                            db.Notifications.Remove(notification);
                            db.SaveChanges();
                        }
                        Notification[] notifications = db.Notifications.Where(r => r.UserId == userId).OrderByDescending(r => r.Id).Take(20).ToArray();
                        return ConvertHelper.GetDataModel(DataType.Notifications, notifications);
                    }
                    else return ConvertHelper.GetDataModel(DataType.Failure, "Invalid username or password");
                }
            }
            catch (Exception ex) { return ConvertHelper.GetDataModel(DataType.Failure, ex.Message); }
        }

        public static DataModel RetryBackTask(NotificationModel notificationModel)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    int userId = AccountDb.ValidUser(notificationModel.UserName, notificationModel.Password, db);
                    if (userId > 0)
                    {
                        var backTask = db.BackTasks.Where(r => r.Id == notificationModel.Id && r.WorkStatus == WorkStatusType.Failed.ToString()).FirstOrDefault();
                        if (backTask != null)
                        {
                            backTask.WorkStatus = WorkStatusType.Wait.ToString();
                            backTask.DateUpdated = DateTime.Now;
                            db.Entry(backTask).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        BackTask[] backTasks = db.BackTasks.Where(r => r.UserId == userId).OrderByDescending(r => r.Id).Take(20).ToArray();
                        return ConvertHelper.GetDataModel(DataType.BackTasks, ToNotifications(backTasks));
                    }
                    else return ConvertHelper.GetDataModel(DataType.Failure, "Invalid username or password");
                }
            }
            catch (Exception ex) { return ConvertHelper.GetDataModel(DataType.Failure, ex.Message); }
        }

        public static DataModel RemoveBackTask(NotificationModel notificationModel)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    int userId = AccountDb.ValidUser(notificationModel.UserName, notificationModel.Password, db);
                    if (userId > 0)
                    {
                        var backTask = db.BackTasks.Where(r => r.Id == notificationModel.Id).FirstOrDefault();
                        if (backTask != null)
                        {
                            db.BackTasks.Remove(backTask);
                            db.SaveChanges();
                        }
                        BackTask[] backTasks = db.BackTasks.Where(r => r.UserId == userId).OrderByDescending(r => r.Id).Take(20).ToArray();
                        return ConvertHelper.GetDataModel(DataType.BackTasks, ToNotifications(backTasks));
                    }
                    else return ConvertHelper.GetDataModel(DataType.Failure, "Invalid username or password");
                }
            }
            catch (Exception ex) { return ConvertHelper.GetDataModel(DataType.Failure, ex.Message); }
        }

        public static Notification[] ToNotifications(BackTask[] backTasks)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    List<Notification> listNotification = new List<Notification>();
                    foreach (var backTask in backTasks)
                    {
                        listNotification.Add(BackTaskDb.GetNotification(backTask, db));
                    }
                    return listNotification.ToArray();
                }
            }
            catch { }
            return null;
        }
    }
}
