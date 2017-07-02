using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class SiteDataDb
    {
        public static ResultModel GetSiteDataId(Site site, SiteDataType siteDataType)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var siteData = db.SiteDatas.Where(r => r.SiteId == site.Id && r.SiteDataType == siteDataType.ToString()).FirstOrDefault();
                    if (siteData == null)
                    {
                        siteData = new SiteData
                        {
                            UserId = site.UserId,
                            SiteId = site.Id,
                            SiteDataType = siteDataType.ToString(),
                            TemplateName = "",
                            IsCompleted = false,
                            DateUpdated = DateTime.Now,
                        };
                        siteData = db.SiteDatas.Add(siteData);
                        db.SaveChanges();
                    }
                    return new ResultModel { Status = true, Result = siteData.Id };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }
        
        public static List<string> LoadTemplate(int userId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.SiteDatas.Where(r => r.UserId == userId && 
                        r.SiteDataType == SiteDataType.Template.ToString()).Select(r => r.TemplateName).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ResultModel ExistsTemplate(SiteData model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    SiteData siteData = null;
                    if (string.IsNullOrEmpty(model.TemplateName))
                    {
                        siteData = db.SiteDatas.Where(r => r.UserId == model.UserId &&
                            r.SiteDataType == SiteDataType.Template.ToString()).FirstOrDefault();
                    }
                    else
                    {
                        siteData = db.SiteDatas.Where(r => r.UserId == model.UserId &&
                            r.SiteDataType == SiteDataType.Template.ToString() &&
                            r.TemplateName == model.TemplateName).FirstOrDefault();
                    }

                    if (siteData == null) return new ResultModel { Status = true, Result = false };
                    else return new ResultModel { Status = true, Result = true };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static ResultModel DeleteSiteData(int siteDataId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    db.Channels.RemoveRange(db.Channels.Where(r => r.SiteDataId == siteDataId).ToArray());
                    db.Schedules.RemoveRange(db.Schedules.Where(r => r.SiteDataId == siteDataId).ToArray());
                    db.TradeCodes.RemoveRange(db.TradeCodes.Where(r => r.SiteDataId == siteDataId).ToArray());
                    db.Doors.RemoveRange(db.Doors.Where(r => r.SiteDataId == siteDataId).ToArray());
                    db.StaffAccesses.RemoveRange(db.StaffAccesses.Where(r => r.SiteDataId == siteDataId).ToArray());
                    db.SystemOptions.RemoveRange(db.SystemOptions.Where(r => r.SiteDataId == siteDataId).ToArray());
                    db.SiteDatas.RemoveRange(db.SiteDatas.Where(r => r.Id == siteDataId).ToArray());
                    db.SaveChanges();

                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static ResultModel LoadParameter(SiteParaModel siteParaModel, Json siteModel)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    if (siteParaModel.SiteData == null || siteParaModel.SiteData.Id == 0)
                    {
                        SiteData siteData = null;
                        if (siteParaModel.SiteDataType == SiteDataType.Template)
                        {
                            siteData = db.SiteDatas.Where(r => r.UserId == siteModel.Site.UserId &&
                                r.SiteDataType == siteParaModel.SiteDataType.ToString() &&
                                r.TemplateName == siteParaModel.SiteData.TemplateName &&
                                r.IsCompleted).FirstOrDefault();
                        }
                        else if (siteParaModel.SiteDataType == SiteDataType.LastSaved ||
                            siteParaModel.SiteDataType == SiteDataType.LastProgrammed ||
                            siteParaModel.SiteDataType == SiteDataType.BackWork)
                        {
                            siteData = db.SiteDatas.Where(r => r.SiteId == siteModel.Site.Id &&
                                r.SiteDataType == siteParaModel.SiteDataType.ToString() &&
                                 r.IsCompleted).FirstOrDefault();
                        }

                        if (siteData == null) return new ResultModel { Status = false, Result = "No existing parameters.", };
                        siteParaModel.SiteData = siteData;
                    }

                    if (siteParaModel.SiteParaType == SiteParaType.Channel)
                    {
                        int curChannelIndex = siteParaModel.Channel.ChannelIndex;
                        siteParaModel.Channel = db.Channels.Where(r => r.SiteDataId == siteParaModel.SiteData.Id &&
                            r.ChannelIndex == siteParaModel.Channel.ChannelIndex).FirstOrDefault();
                        if (siteParaModel.Channel == null)
                        {
                            // modified by me
                            siteParaModel.Channel = new Channel { ChannelIndex = curChannelIndex };
                            //siteParaModel.Channel = new Channel { ChannelIndex = siteParaModel.Channel.ChannelIndex };
                        }
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.Schedule)
                    {
                        int curScheduleIndex = siteParaModel.Schedule.ScheduleIndex;
                        siteParaModel.Schedule = db.Schedules.Where(r => r.SiteDataId == siteParaModel.SiteData.Id &&
                            r.ScheduleIndex == siteParaModel.Schedule.ScheduleIndex).FirstOrDefault();
                        if (siteParaModel.Schedule == null)
                        {
                            // modified by me
                            siteParaModel.Schedule = new Schedule { ScheduleIndex = curScheduleIndex };
                            //siteParaModel.Schedule = new Schedule { ScheduleIndex = siteParaModel.Schedule.ScheduleIndex };
                        }
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.TradeCode)
                    {
                        int curTradeCodeIndex = siteParaModel.TradeCode.TradeCodeIndex;
                        siteParaModel.TradeCode = db.TradeCodes.Where(r => r.SiteDataId == siteParaModel.SiteData.Id &&
                            r.TradeCodeIndex == siteParaModel.TradeCode.TradeCodeIndex).FirstOrDefault();
                        if (siteParaModel.TradeCode == null)
                        {
                            // modified by me
                            siteParaModel.TradeCode = new TradeCode { TradeCodeIndex = curTradeCodeIndex };
                            //siteParaModel.TradeCode = new TradeCode { TradeCodeIndex = siteParaModel.TradeCode.TradeCodeIndex };
                        }
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.Door)
                    {
                        int curDoorIndex = siteParaModel.Door.DoorIndex;
                        siteParaModel.Door = db.Doors.Where(r => r.SiteDataId == siteParaModel.SiteData.Id &&
                            r.DoorIndex == siteParaModel.Door.DoorIndex).FirstOrDefault();
                        if (siteParaModel.Door == null)
                        {
                            // modified by me
                            siteParaModel.Door = new Door { DoorIndex = curDoorIndex };
                            //siteParaModel.Door = new Door { DoorIndex = siteParaModel.Door.DoorIndex };
                        }
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.StaffAccess)
                    {
                        int curStaffAccessIndex = siteParaModel.StaffAccess.StaffAccessIndex;
                        siteParaModel.StaffAccess = db.StaffAccesses.Where(r => r.SiteDataId == siteParaModel.SiteData.Id &&
                            r.StaffAccessIndex == siteParaModel.StaffAccess.StaffAccessIndex).FirstOrDefault();
                        if (siteParaModel.StaffAccess == null)
                        {
                            // modified by me
                            siteParaModel.StaffAccess = new StaffAccess { StaffAccessIndex = curStaffAccessIndex };
                            //siteParaModel.StaffAccess = new StaffAccess { StaffAccessIndex = siteParaModel.StaffAccess.StaffAccessIndex };
                        }
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.SystemOption)
                    {
                        //int curStaffAccessIndex = siteParaModel.StaffAccess.StaffAccessIndex;
                        siteParaModel.SystemOption = db.SystemOptions.Where(r => r.SiteDataId == siteParaModel.SiteData.Id).FirstOrDefault();
                        if (siteParaModel.SystemOption == null) siteParaModel.SystemOption = new SystemOption();
                        int a = 1;
                    }

                    return new ResultModel { Status = true, Result = siteParaModel };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; }
        }

        public static ResultModel SaveParameter(SiteParaModel siteParaModel, Json siteModel)
        {
            try
            {
                using(var db = new DESEntities())
                {
                    if (siteParaModel.SiteData == null) siteParaModel.SiteData = new SiteData { TemplateName = "" };

                    if (siteParaModel.SiteData.Id == 0)
                    {
                        siteParaModel.SiteData.IsCompleted = false;
                        siteParaModel.SiteData.SiteId = siteModel.Site.Id;
                        siteParaModel.SiteData.UserId = siteModel.Site.UserId;
                        siteParaModel.SiteData.SiteDataType = siteParaModel.SiteDataType.ToString();

                        SiteData siteData = null;
                        if (siteParaModel.SiteDataType == SiteDataType.Template)
                        {
                            siteData = db.SiteDatas.Where(r => r.UserId == siteModel.Site.UserId &&
                                r.SiteDataType == siteParaModel.SiteDataType.ToString() &&
                                r.TemplateName == siteParaModel.SiteData.TemplateName).FirstOrDefault();
                        }
                        else if (siteParaModel.SiteDataType == SiteDataType.LastSaved ||
                            siteParaModel.SiteDataType == SiteDataType.LastProgrammed ||
                            siteParaModel.SiteDataType == SiteDataType.BackWork)
                        {
                            siteData = db.SiteDatas.Where(r => r.SiteId == siteModel.Site.Id &&
                                r.SiteDataType == siteParaModel.SiteDataType.ToString()).FirstOrDefault();
                        }

                        if (siteData == null)
                        {
                            siteParaModel.SiteData.IsCompleted = false;
                            siteParaModel.SiteData.DateUpdated = DateTime.Now;
                            siteData = db.SiteDatas.Add(siteParaModel.SiteData);
                        }
                        else
                        {
                            siteParaModel.SiteData.IsCompleted = false;
                            siteData.TemplateName = siteParaModel.SiteData.TemplateName;
                            siteData.DateUpdated = DateTime.Now;
                            db.Entry(siteData).State = System.Data.Entity.EntityState.Modified;
                        }
                        db.SaveChanges();
                        siteParaModel.SiteData = siteData;
                    }

                    if (siteParaModel.SiteParaType == SiteParaType.Channel)
                    {
                        siteParaModel.Channel.SiteDataId = siteParaModel.SiteData.Id;
                        ResultModel resultModel = ChannelDb.Save(db, siteParaModel.Channel);
                        if (!resultModel.Status) return resultModel;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.Schedule)
                    {
                        siteParaModel.Schedule.SiteDataId = siteParaModel.SiteData.Id;
                        ResultModel resultModel = ScheduleDb.Save(db, siteParaModel.Schedule);
                        if (!resultModel.Status) return resultModel;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.TradeCode)
                    {
                        siteParaModel.TradeCode.SiteDataId = siteParaModel.SiteData.Id;
                        ResultModel resultModel = TradeCodeDb.Save(db, siteParaModel.TradeCode);
                        if (!resultModel.Status) return resultModel;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.Door)
                    {
                        siteParaModel.Door.SiteDataId = siteParaModel.SiteData.Id;
                        ResultModel resultModel = DoorDb.Save(db, siteParaModel.Door);
                        if (!resultModel.Status) return resultModel;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.StaffAccess)
                    {
                        siteParaModel.StaffAccess.SiteDataId = siteParaModel.SiteData.Id;
                        ResultModel resultModel = StaffAccessDb.Save(db, siteParaModel.StaffAccess);
                        if (!resultModel.Status) return resultModel;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.SystemOption)
                    {
                        siteParaModel.SystemOption.SiteDataId = siteParaModel.SiteData.Id;
                        ResultModel resultModel = SystemOptionDb.Save(db, siteParaModel.SystemOption);
                        if (!resultModel.Status) return resultModel;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.End)
                    {
                        if (siteParaModel.SiteDataType == SiteDataType.BackWork)
                        {
                            var backTask = new BackTask
                            {
                                UserId = siteModel.Site.UserId,
                                DeviceId = siteModel.Site.DeviceId,
                                SiteDataId = siteParaModel.SiteData.Id,
                                WorkType = SiteDataType.LastProgrammed.ToString(),
                                WorkStatus = WorkStatusType.Wait.ToString(),
                                WorkItem = SiteParaType.Channel.ToString(),
                                WorkIndex = 1,
                                Description = "",
                            };
                            BackTaskDb.AddBackTask(backTask);
                        }

                        SiteData siteData = db.SiteDatas.Where(r => r.Id == siteParaModel.SiteData.Id).FirstOrDefault();
                        if (siteData != null)
                        {
                            siteData.IsCompleted = true;
                            siteData.DateUpdated = DateTime.Now;
                            db.Entry(siteData).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            siteParaModel.SiteData = siteData;
                        }
                    }
                    return new ResultModel { Status = true, Result = siteParaModel };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; }
        }
    }
}
