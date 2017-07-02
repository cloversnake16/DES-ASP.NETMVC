using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class BackTaskDb
    {
        public static ResultModel ExistsBackTask(string deviceId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var backTask = db.BackTasks.Where(r => r.DeviceId == deviceId && r.WorkType == SiteDataType.LastProgrammed.ToString() &&
                        (r.WorkStatus == WorkStatusType.Wait.ToString() || r.WorkStatus == WorkStatusType.Progress.ToString())).FirstOrDefault();
                    if (backTask != null) return new ResultModel { Status = true, Result = true };
                    else return new ResultModel { Status = true, Result = false };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public static BackTask GetBackTask(int backTaskId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.BackTasks.Where(r => r.Id == backTaskId && (r.WorkStatus == WorkStatusType.Wait.ToString() || r.WorkStatus == WorkStatusType.Progress.ToString())).FirstOrDefault();
                }
            }
            catch { }
            return null;
        }

        public static BackTask GetBackTask(string deviceId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.BackTasks.Where(r => r.DeviceId == deviceId && (r.WorkStatus == WorkStatusType.Wait.ToString() || r.WorkStatus == WorkStatusType.Progress.ToString())).FirstOrDefault();
                }
            }
            catch { }
            return null;
        }

        public static ResultModel AddBackTask(BackTask backTask)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    DateTime now = DateTime.Now;
                    backTask.DateCreated = now;
                    backTask.DateUpdated = now;
                    db.BackTasks.Add(backTask);
                    db.SaveChanges();
                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static void UpdateBackTask(BackTask bt)
        {
            try
            {
                if (bt == null) return;

                using (var db = new DESEntities())
                {
                    var backTask = db.BackTasks.Where(r => r.Id == bt.Id).FirstOrDefault();
                    if (backTask != null)
                    {
                        backTask.WorkStatus = bt.WorkStatus;
                        backTask.WorkItem = bt.WorkItem;
                        backTask.WorkIndex = bt.WorkIndex;
                        backTask.Description = bt.Description;
                        backTask.DateUpdated = DateTime.Now;
                        db.Entry(backTask).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        if (backTask.WorkStatus == WorkStatusType.Completed.ToString() || backTask.WorkStatus == WorkStatusType.Failed.ToString())
                        {
                            Notification notification = GetNotification(backTask);
                            NotificationDb.AddNotification(notification, db);
                        }

                        if (backTask.WorkStatus == WorkStatusType.Completed.ToString())
                        {
                            var site = db.Sites.Where(r => r.DeviceId == backTask.DeviceId).FirstOrDefault();
                            if (site != null)
                            {
                                var siteData = db.SiteDatas.Where(r => r.SiteId == site.Id && r.SiteDataType == SiteDataType.LastProgrammed.ToString()).FirstOrDefault();
                                if (siteData != null)
                                {
                                    siteData.IsCompleted = true;
                                    siteData.DateUpdated = DateTime.Now;
                                    db.Entry(siteData).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static ResultModel RetryBackTask(int backTaskId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var backTask = db.BackTasks.Where(r => r.Id == backTaskId && r.WorkStatus == WorkStatusType.Failed.ToString()).FirstOrDefault();
                    if (backTask != null)
                    {
                        backTask.WorkStatus = WorkStatusType.Wait.ToString();
                        backTask.DateUpdated = DateTime.Now;
                        db.Entry(backTask).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return new ResultModel { Status = true, Result = "Restarted background task successfully." };
                    }
                    return new ResultModel { Status = false, Result = "The failed task can be retried." };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static ResultModel DeleteBackTask(int backTaskId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var backTask = db.BackTasks.Where(r => r.Id == backTaskId).FirstOrDefault();
                    if (backTask != null)
                    {
                        db.BackTasks.Remove(backTask);
                        db.SaveChanges();
                    }
                    return new ResultModel { Status = true, Result = "Deleted background task successfully." };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static BackTask NextBackTask(BackTask backTask)
        {
            try
            {
                if (backTask.WorkType == SiteDataType.ProgramStaff.ToString())
                {
                    backTask.WorkIndex++;
                    using (var db = new DESEntities())
                    {
                        var site = db.Sites.Where(r => r.DeviceId == backTask.DeviceId).FirstOrDefault();
                        if (site != null)
                        {
                            var count = db.StaffGroupSites.Where(r => r.SiteId == site.Id).Count();
                            if (backTask.WorkIndex >= count)
                            {
                                backTask.WorkStatus = WorkStatusType.Completed.ToString();
                            }
                        }
                    }
                }
                else
                {
                    if (backTask.WorkItem == SiteParaType.Channel.ToString())
                    {
                        backTask.WorkStatus = WorkStatusType.Progress.ToString();
                        if (backTask.WorkIndex < 256) backTask.WorkIndex++;
                        else
                        {
                            backTask.WorkItem = SiteParaType.Schedule.ToString();
                            backTask.WorkIndex = 1;
                        }
                    }
                    else if (backTask.WorkItem == SiteParaType.Schedule.ToString())
                    {
                        if (backTask.WorkIndex < 32) backTask.WorkIndex++;
                        else
                        {
                            backTask.WorkItem = SiteParaType.TradeCode.ToString();
                            backTask.WorkIndex = 1;
                        }
                    }
                    else if (backTask.WorkItem == SiteParaType.TradeCode.ToString())
                    {
                        if (backTask.WorkIndex < 16) backTask.WorkIndex++;
                        else
                        {
                            backTask.WorkItem = SiteParaType.Door.ToString();
                            backTask.WorkIndex = 1;
                        }
                    }
                    else if (backTask.WorkItem == SiteParaType.Door.ToString())
                    {
                        if (backTask.WorkIndex < 128) backTask.WorkIndex++;
                        else
                        {
                            backTask.WorkItem = SiteParaType.StaffAccess.ToString();
                            backTask.WorkIndex = 1;
                        }
                    }
                    else if (backTask.WorkItem == SiteParaType.StaffAccess.ToString())
                    {
                        if (backTask.WorkIndex < 32) backTask.WorkIndex++;
                        else
                        {
                            backTask.WorkItem = SiteParaType.SystemOption.ToString();
                            backTask.WorkIndex = 1;
                        }
                    }
                    else if (backTask.WorkItem == SiteParaType.SystemOption.ToString())
                    {
                        backTask.WorkStatus = WorkStatusType.Completed.ToString();
                    }
                }

                return backTask;
            }
            catch { }
            return null;
        }

        public static SiteParaModel GetSiteParaModel(BackTask backTask)
        {
            try
            {
                SiteParaModel siteParaModel = new SiteParaModel();

                if(backTask.WorkType == SiteDataType.ProgramStaff.ToString())
                {
                    siteParaModel.SiteParaType = SiteParaType.Channel;
                
                    using(var db = new DESEntities())
                    {
                        var site = db.Sites.Where(r => r.DeviceId == backTask.DeviceId).FirstOrDefault();
                        if(site != null)
                        {
                            var count = db.StaffGroupSites.Where(r => r.SiteId == site.Id).Count();
                            if(backTask.WorkIndex < count)
                            {
                                var staffGroupSite = db.StaffGroupSites.Where(r => r.SiteId == site.Id).OrderBy(r => r.Id).Skip(backTask.WorkIndex).Take(1).FirstOrDefault();
                                if (staffGroupSite != null)
                                {
                                    var staffGroup = db.StaffGroups.Where(r => r.Id == staffGroupSite.StaffGroupId).FirstOrDefault();
                                    if(staffGroup != null)
                                    {
                                        siteParaModel.Channel = new Channel();
                                        siteParaModel.Channel.ChannelIndex = staffGroupSite.ChannelIndex;
                                        siteParaModel.Channel.Flat = 0;
                                        siteParaModel.Channel.PPP = "";
                                        siteParaModel.Channel.Door1 = staffGroup.Door1;
                                        siteParaModel.Channel.Door2 = staffGroup.Door2;
                                        siteParaModel.Channel.Tag1 = staffGroup.Tag1;
                                        siteParaModel.Channel.Tag2 = staffGroup.Tag2;
                                        siteParaModel.Channel.Tag3 = staffGroup.Tag3;
                                        siteParaModel.Channel.Tag4 = staffGroup.Tag4;
                                        siteParaModel.Channel.Tag5 = staffGroup.Tag5;
                                        siteParaModel.Channel.Tag6 = staffGroup.Tag6;
                                        siteParaModel.Channel.Tag7 = staffGroup.Tag7;
                                        siteParaModel.Channel.Tag8 = staffGroup.Tag8;
                                        return siteParaModel;
                                    }
                                }
                            }
                            else backTask.WorkStatus = WorkStatusType.Completed.ToString();
                        }
                    }
                }
                else
                {
                    siteParaModel.SiteData = new SiteData { Id = backTask.SiteDataId, };
                    siteParaModel.SiteParaType = (SiteParaType)Enum.Parse(typeof(SiteParaType), backTask.WorkItem);

                    if (siteParaModel.SiteParaType == SiteParaType.Channel)
                    {
                        siteParaModel.Channel = new Channel { ChannelIndex = backTask.WorkIndex };
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.Schedule)
                    {
                        siteParaModel.Schedule = new Schedule { ScheduleIndex = backTask.WorkIndex };
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.TradeCode)
                    {
                        siteParaModel.TradeCode = new TradeCode { TradeCodeIndex = backTask.WorkIndex };
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.Door)
                    {
                        siteParaModel.Door = new Door { DoorIndex = backTask.WorkIndex };
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.StaffAccess)
                    {
                        siteParaModel.StaffAccess = new StaffAccess { StaffAccessIndex = backTask.WorkIndex };
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.SystemOption)
                    {
                        siteParaModel.SystemOption = new SystemOption();
                    }
                }

                return siteParaModel;
            }
            catch { }
            return null;
        }

        public static List<Notification> GetNotifications(int userId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    List<Notification> listNotification = new List<Notification>();
                    var backTasks = db.BackTasks.Where(r => r.UserId == userId && r.WorkType == SiteDataType.LastProgrammed.ToString()).OrderByDescending(r => r.Id).ToArray();
                    foreach (var backTask in backTasks)
                    {
                        Notification notification = GetNotification(backTask, db);
                        listNotification.Add(notification);
                    }
                    return listNotification;
                }
            }
            catch { }
            return null;
        }

        public static Notification GetNotification(BackTask backTask, DESEntities db = null)
        {
            try
            {
                if (backTask == null) return null;

                if (db == null)
                {
                    using (var entity = new DESEntities())
                    {
                        return GetNotification(backTask, entity);
                    }
                }
                else 
                {
                    var site = db.Sites.Where(r => r.DeviceId == backTask.DeviceId).FirstOrDefault();
                    string title = "Program to " + site.SiteName;

                    string contents = "";
                    if (backTask.WorkStatus == WorkStatusType.Wait.ToString())
                    {
                        contents = "Task is waiting for its turn.";
                    }
                    else if (backTask.WorkStatus == WorkStatusType.Completed.ToString())
                    {
                        contents = "Task was completed.";
                    }
                    else if (backTask.WorkStatus == WorkStatusType.Progress.ToString())
                    {
                        contents = "Programing " + backTask.WorkItem + backTask.WorkIndex + "...";
                    }
                    else if (backTask.WorkStatus == WorkStatusType.Failed.ToString())
                    {
                        contents = "Task was failed." + backTask.Description;
                    }

                    return new Notification
                    {
                        Id = backTask.Id,
                        UserId = backTask.UserId,
                        Title = title,
                        Contents = contents,
                        DateUpdated = backTask.DateCreated,
                    };
                }
            }
            catch { }
            return null;
        }
    }
}
