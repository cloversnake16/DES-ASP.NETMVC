using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Configuration;
using System.Threading.Tasks;
using DES.Models;
using DESCore.Models;
using DESCore.Database;
using DESCore.Helpers;
using DESCore.DesCommands;

namespace DES.Controllers
{
    public class SiteController : Controller
    {
        public ActionResult Index()
        {
            if (Session["SiteDataViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            return View((SiteDataViewModel)Session["SiteDataViewModel"]);
        }

        public ActionResult SearchEventLog(LocalEventLogQueryModel query)
        {
            if (Session["LocalEventLogViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            LocalEventLogViewModel eventLogViewModel = (LocalEventLogViewModel)Session["LocalEventLogViewModel"];

            eventLogViewModel.Query.QueryKey = query.QueryKey;
            eventLogViewModel.Query.QueryValue = query.QueryValue;
            eventLogViewModel.Query.FromDateTime = query.FromDateTime;
            eventLogViewModel.Query.ToDateTime = query.ToDateTime;
            eventLogViewModel.Events = new EventLogDb().GetLocalEventLogs(eventLogViewModel.Query);

            return RedirectToAction("LocalEventLog");
        }

        public ActionResult ClearEventLog()
        {
            if (Session["LocalEventLogViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            LocalEventLogViewModel eventLogViewModel = (LocalEventLogViewModel)Session["LocalEventLogViewModel"];

            eventLogViewModel.Query.QueryKey = null;
            eventLogViewModel.Query.QueryValue = null;
            eventLogViewModel.Query.FromDateTime = DateTime.Today;
            eventLogViewModel.Query.ToDateTime = DateTime.Today;

            return RedirectToAction("LocalEventLog");
        }

        public ActionResult LocalEventLog()
        {
            LocalEventLogViewModel model = null;
            try
            {
                if (Session["LocalEventLogViewModel"] == null)
                {
                    if (Session["SiteModel"] == null) return RedirectToAction("Index");
                    Json siteModel = (Json)Session["SiteModel"];

                    LocalEventLogQueryModel query = new LocalEventLogQueryModel
                    {
                        QueryItems = Enum.GetNames(typeof(LocalEventLogQueryType)),
                        QueryKey = null,
                        QueryValue = null,
                        FromDateTime = DateTime.Today,
                        ToDateTime = DateTime.Today,
                        SiteModel = siteModel,
                    };

                    model = new LocalEventLogViewModel
                    {
                        Events = new EventLogDb().GetLocalEventLogs(query),
                        Query = query,
                    };

                    Session["LocalEventLogViewModel"] = model;
                }
                else model = (LocalEventLogViewModel)Session["LocalEventLogViewModel"];
            }
            catch (Exception) { }

            return View(model);
        }

        public ActionResult DetailedEventLog(int EventId)
        {
            if (Session["LocalEventLogViewModel"] == null) return RedirectToAction("LocalEventLog");
            LocalEventLogViewModel localEventLogViewModel = (LocalEventLogViewModel)Session["LocalEventLogViewModel"];

            EventLogModel eventLog = localEventLogViewModel.Events.Where(r => r.EventLog.Id == EventId).FirstOrDefault();
            return View(eventLog);
        }

        public ActionResult RemoteEventLog()
        {
            RemoteEventLogViewModel model = null;
            try
            {
                if (Session["RemoteEventLogViewModel"] == null)
                {
                    if (Session["SiteModel"] == null) return RedirectToAction("Index");
                    Json siteModel = (Json)Session["SiteModel"];

                    RemoteEventLogQueryModel query = new RemoteEventLogQueryModel
                    {
                        QueryItems = Enum.GetNames(typeof(RemoteEventLogQueryType)),
                        FromDateTime = DateTime.Today,
                        ToDateTime = DateTime.Today,
                        SiteModel = siteModel,
                    };

                    model = new RemoteEventLogViewModel
                    {
                        Events = new EventLogDb().GetRemoteEvents(query),
                        Query = query,
                    };

                    Session["RemoteEventLogViewModel"] = model;
                }
                else model = (RemoteEventLogViewModel)Session["RemoteEventLogViewModel"];
            }
            catch (Exception) { }

            return View(model);
        }

        public ActionResult DetailedRemoteEventLog(int EventId)
        {
            if (Session["RemoteEventLogViewModel"] == null) return RedirectToAction("RemoteEventLog");
            RemoteEventLogViewModel remoteEventLogViewModel = (RemoteEventLogViewModel)Session["RemoteEventLogViewModel"];

            RemoteSiteEventModel eventLog = remoteEventLogViewModel.Events.Where(r => r.RemoteSiteEventLog.Id == EventId).FirstOrDefault();
            return View(eventLog);
        }

        public ActionResult SearchRemoteEventLog(RemoteEventLogQueryModel query)
        {
            if (Session["RemoteEventLogViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            RemoteEventLogViewModel eventLogViewModel = (RemoteEventLogViewModel)Session["RemoteEventLogViewModel"];

            eventLogViewModel.Query.QueryKey = query.QueryKey;
            eventLogViewModel.Query.FromDateTime = query.FromDateTime;
            eventLogViewModel.Query.ToDateTime = query.ToDateTime;
            eventLogViewModel.Events = new EventLogDb().GetRemoteEvents(eventLogViewModel.Query);

            return RedirectToAction("RemoteEventLog");
        }

        public ActionResult ClearRemoteEventLog()
        {
            if (Session["RemoteEventLogViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            RemoteEventLogViewModel eventLogViewModel = (RemoteEventLogViewModel)Session["RemoteEventLogViewModel"];

            eventLogViewModel.Query.QueryKey = null;
            eventLogViewModel.Query.FromDateTime = DateTime.Today;
            eventLogViewModel.Query.ToDateTime = DateTime.Today;

            return RedirectToAction("RemoteEventLog");
        }

        [HttpPost]
        public JsonResult ExistsTemplate(string templateName)
        {
            if (Session["UserModel"] == null) return Json(null, JsonRequestBehavior.AllowGet);
            UserModel user = (UserModel)Session["UserModel"];

            SiteData siteData = new SiteData { UserId = user.User.Id, TemplateName = templateName };

            ResultModel result = SiteDataDb.ExistsTemplate(siteData);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ExistsBackTask()
        {
            try
            {
                Json siteModel = (Json)Session["SiteModel"];
                ResultModel result = BackTaskDb.ExistsBackTask(siteModel.Site.DeviceId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new ResultModel{ Status = false, Result = ex.Message }); }
        }

        [HttpPost]
        public JsonResult DeleteSiteData(int SiteDataId)
        {
            ResultModel result = SiteDataDb.DeleteSiteData(SiteDataId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadParameter(SiteParaViewModel siteParaViewModel)
        {
            try
            {
                ResultModel result = null;
                Json siteModel = null;
                if (Session["SiteModel"] != null) siteModel = (Json)Session["SiteModel"];
                                
                if (siteParaViewModel.SiteDataType == SiteDataType.MoveSite)
                {
                    SiteDataViewModel siteDataViewModel = null;
                    if (Session["SiteDataViewModel"] != null) siteDataViewModel = (SiteDataViewModel)Session["SiteDataViewModel"];

                    if (siteParaViewModel.SiteParaType == SiteParaType.Begin)
                    {
                        Session["SiteModel"] = SiteDb.GetSite(siteParaViewModel.SiteData.SiteId);
                        siteModel = (Json)Session["SiteModel"];
                        Session["SiteDataViewModel"] = NewSiteDataViewModel();
                        siteDataViewModel = (SiteDataViewModel)Session["SiteDataViewModel"];

                        Session["Template"] = SiteDataDb.LoadTemplate(siteModel.Site.UserId);
                        Session["ReservedChannels"] = SettingDb.GetReservedChannelIndics();

                        var doorDescriptions = SettingDb.GetDoorDescriptions();
                        siteDataViewModel.DoorDescriptions = new string[128];
                        if (doorDescriptions != null)
                        {
                            foreach (var doorDescription in doorDescriptions)
                            {
                                siteDataViewModel.DoorDescriptions[doorDescription.Index] = doorDescription.Contents;
                            }
                        }
                        siteParaViewModel.SiteDataType = SiteDataType.LastProgrammed;
                        result = SiteDataDb.LoadParameter(GetSiteParaModel(siteParaViewModel), siteModel);
                        siteParaViewModel.SiteDataType = SiteDataType.MoveSite;
                    }
                    else if (siteParaViewModel.SiteParaType != SiteParaType.End)
                    {
                        siteParaViewModel.SiteDataType = SiteDataType.LastProgrammed;
                        result = SiteDataDb.LoadParameter(GetSiteParaModel(siteParaViewModel), siteModel);
                        siteParaViewModel.SiteDataType = SiteDataType.MoveSite;
                        
                        if (result.Status)
                        {
                            SiteParaViewModel model = GetSiteParaViewModel((SiteParaModel)result.Result);
                            if (siteParaViewModel.SiteParaType == SiteParaType.Channel)
                            {
                                siteDataViewModel.ChannelViewModels[model.ChannelViewModel.ChannelIndex - 1] = model.ChannelViewModel;
                            }
                            else if (siteParaViewModel.SiteParaType == SiteParaType.Schedule)
                            {
                                siteDataViewModel.ScheduleViewModels[model.ScheduleViewModel.ScheduleIndex - 1] = model.ScheduleViewModel;
                            }
                            else if (siteParaViewModel.SiteParaType == SiteParaType.TradeCode)
                            {
                                siteDataViewModel.TradeCodeViewModels[model.TradeCodeViewModel.TradeCodeIndex - 1] = model.TradeCodeViewModel;
                            }
                            else if (siteParaViewModel.SiteParaType == SiteParaType.Door)
                            {
                                siteDataViewModel.DoorViewModels[model.DoorViewModel.DoorIndex - 1] = model.DoorViewModel;
                            }
                            else if (siteParaViewModel.SiteParaType == SiteParaType.StaffAccess)
                            {
                                siteDataViewModel.StaffAccessViewModels[model.StaffAccessViewModel.StaffAccessIndex - 1] = model.StaffAccessViewModel;
                            }
                            else if (siteParaViewModel.SiteParaType == SiteParaType.SystemOption)
                            {
                                siteDataViewModel.SystemOptionViewModel = model.SystemOptionViewModel;
                            }

                            siteDataViewModel.IsNew = false;
                        }
                        else
                        {
                            //modify by me
                            //return Json(result);
                        }
                    }
                    return Json(new ResultModel { Status = result.Status, Result = siteParaViewModel, });
                }

                if (siteParaViewModel.SiteParaType == SiteParaType.Channel)
                {
                    if (Session["ReservedChannels"] != null)
                    {
                        int[] reservedChannels = (int[])Session["ReservedChannels"];
                        if (reservedChannels.Contains(siteParaViewModel.ChannelViewModel.ChannelIndex - 1))
                        {
                            return Json(new ResultModel { Status = true, Result = siteParaViewModel, });
                        }
                    }
                }
                
                if (siteParaViewModel.SiteDataType == SiteDataType.Retrieve)
                {
                    result = CommandHelper.LoadParameter(GetSiteParaModel(siteParaViewModel), siteModel);
                }
                else
                {
                    result = SiteDataDb.LoadParameter(GetSiteParaModel(siteParaViewModel), siteModel);
                }

                if (result.Status)
                {
                    result.Result = GetSiteParaViewModel((SiteParaModel)result.Result);

                    SiteDataViewModel siteDataViewModel = (SiteDataViewModel)Session["SiteDataViewModel"];
                    siteDataViewModel.IsNew = false;
                }
                return Json(result);
            }
            catch (Exception ex) { return Json(new ResultModel {Status = false, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult SaveParameter(SiteParaViewModel siteParaViewModel)
        {
            try
            {
                if (siteParaViewModel.SiteParaType == SiteParaType.Channel)
                {
                    if (Session["ReservedChannels"] != null)
                    {
                        int[] reservedChannels = (int[])Session["ReservedChannels"];
                        if (reservedChannels.Contains(siteParaViewModel.ChannelViewModel.ChannelIndex - 1))
                        {
                            return Json(new ResultModel { Status = true, Result = siteParaViewModel, });
                        }
                    }

                    if (siteParaViewModel.SiteDataType == SiteDataType.LastProgrammed ||
                        siteParaViewModel.SiteDataType == SiteDataType.BackWork)
                    {
                        if (!siteParaViewModel.ChannelViewModel.Selected)
                        {
                            return Json(new ResultModel { Status = true, Result = siteParaViewModel, });
                        }
                    }
                }

                UserModel userModel = (UserModel)Session["UserModel"];
                SiteDataViewModel siteDataViewModel = (SiteDataViewModel)Session["SiteDataViewModel"];
                if (siteDataViewModel.IsNew && userModel.UserTypeName != "SysAdmin" && siteDataViewModel.IsNew && userModel.UserTypeName != "Enginner")
                {
                    return Json(new ResultModel { Status = false, Result = "Data is new one. New data can be saved only by SysAdmin or Engineer.", });
                }

                Json siteModel = (Json)Session["SiteModel"];
                ResultModel result = new ResultModel { Status = true };
                if (siteParaViewModel.SiteDataType == SiteDataType.LastProgrammed)
                {
                    result = CommandHelper.SaveParameter(GetSiteParaModel(siteParaViewModel), siteModel);
                }

                if (result.Status)
                {
                    result = SiteDataDb.SaveParameter(GetSiteParaModel(siteParaViewModel), siteModel);
                }

                if (result.Status)
                {
                    siteParaViewModel = GetSiteParaViewModel((SiteParaModel)result.Result);
                    if (siteParaViewModel.SiteDataType == SiteDataType.Template &&
                        siteParaViewModel.SiteParaType == SiteParaType.End)
                    {
                        List<string> templates = templates = new List<string>(); 
                        if (Session["Template"] != null) templates = (List<string>)Session["Template"];
                        templates.Add(siteParaViewModel.SiteData.TemplateName);
                        Session["Template"] = templates;
                    }
                    result.Result = siteParaViewModel;
                }
                return Json(result);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult RetrieveDateTime()
        {
            if (Session["SiteModel"] == null) new ResultModel { Status = false, Result = "Disconnect" };
            Json siteModel = (Json)Session["SiteModel"];
            string deviceId = siteModel.Site.DeviceId;

            ResultModel result = CommandHelper.RetrieveDateTime(deviceId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProgramDateTime()
        {
            if (Session["SiteModel"] == null) new ResultModel { Status = false, Result = "Disconnect" };
            Json siteModel = (Json)Session["SiteModel"];
            string deviceId = siteModel.Site.DeviceId;

            ResultModel result = CommandHelper.ProgramDateTime(deviceId, DateTime.Now.ToUniversalTime());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProgramEventsEnable(EventEnableModel model)
        {
            if (Session["SiteModel"] == null) new ResultModel { Status = false, Result = "Disconnect" };
            Json siteModel = (Json)Session["SiteModel"];
            string deviceId = siteModel.Site.DeviceId;

            ResultModel result = CommandHelper.ProgramEventsEnable(deviceId, model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private SiteDataViewModel NewSiteDataViewModel()
        {
            SiteDataViewModel siteDataViewModel = new SiteDataViewModel { IsNew = true, };

            siteDataViewModel.ChannelViewModels = new ChannelViewModel[256];
            for (int i = 0; i < 256; i++)
            {
                siteDataViewModel.ChannelViewModels[i] = new ChannelViewModel
                {
                    ChannelIndex = i + 1,
                    PPP = "",
                };

                siteDataViewModel.ChannelViewModels[i].Doors = new bool[8][];
                siteDataViewModel.ChannelViewModels[i].DoorVisibles = new bool[8];
                for (int j = 0; j < 8; j++)
                {
                    siteDataViewModel.ChannelViewModels[i].Doors[j] = new bool[16];
                    if (j < 1) siteDataViewModel.ChannelViewModels[i].DoorVisibles[j] = true;
                    else siteDataViewModel.ChannelViewModels[i].DoorVisibles[j] = false;
                    for (int k = 0; k < 16; k++) siteDataViewModel.ChannelViewModels[i].Doors[j][k] = false;
                }

                siteDataViewModel.ChannelViewModels[i].Tags = new int[8];
            }

            siteDataViewModel.ScheduleViewModels = new ScheduleViewModel[32];
            for (int i = 0; i < 32; i++)
            {
                siteDataViewModel.ScheduleViewModels[i] = new ScheduleViewModel { ScheduleIndex = i + 1, Day1 = new bool[8], Day2 = new bool[8],};
            }

            siteDataViewModel.TradeCodeViewModels = new TradeCodeViewModel[16];
            for (int i = 0; i < 16; i++) siteDataViewModel.TradeCodeViewModels[i] = new TradeCodeViewModel { TradeCodeIndex = i + 1, };

            siteDataViewModel.DoorViewModels = new DoorViewModel[128];
            for (int i = 0; i < 128; i++) siteDataViewModel.DoorViewModels[i] = new DoorViewModel { DoorIndex = i + 1, };

            siteDataViewModel.StaffAccessViewModels = new StaffAccessViewModel[32];
            for (int i = 0; i < 32; i++) siteDataViewModel.StaffAccessViewModels[i] = new StaffAccessViewModel { StaffAccessIndex = i + 1, };

            siteDataViewModel.SystemOptionViewModel = new SystemOptionViewModel();

            return siteDataViewModel;
        }

        private SiteParaViewModel GetSiteParaViewModel(SiteParaModel model)
        {
            return new SiteParaViewModel
            {
                SiteData = model.SiteData,
                SiteDataType = model.SiteDataType,
                SiteParaType = model.SiteParaType,
                All = model.All,
                Next = model.Next,
                ChannelViewModel = GetChannelViewModel(model.Channel),
                ScheduleViewModel = GetScheduleViewModel(model.Schedule),
                TradeCodeViewModel = GetTradeCodeViewModel(model.TradeCode),
                DoorViewModel = GetDoorViewModel(model.Door),
                StaffAccessViewModel = GetStaffAccessViewModel(model.StaffAccess),
                SystemOptionViewModel = GetSystemOptionViewModel(model.SystemOption),
            };
        }

        private SiteParaModel GetSiteParaModel(SiteParaViewModel model)
        {
            SiteParaModel siteParaModel = new SiteParaModel
            {
                SiteData = model.SiteData,
                SiteDataType = model.SiteDataType,
                SiteParaType = model.SiteParaType,
                All = model.All,
                Next = model.Next,
                Channel = GetChannel(model.ChannelViewModel),
                Schedule = GetSchedule(model.ScheduleViewModel),
                TradeCode = GetTradeCode(model.TradeCodeViewModel),
                Door = GetDoor(model.DoorViewModel),
                StaffAccess = GetStaffAccess(model.StaffAccessViewModel),
                SystemOption = GetSystemOption(model.SystemOptionViewModel),
            };                
            return siteParaModel;
        }

        private ChannelViewModel GetChannelViewModel(Channel model)
        {
            if (model == null) return null;

            ChannelViewModel channelViewModel = new ChannelViewModel
            {
                ChannelIndex = model.ChannelIndex,
                Flat = model.Flat,
                PPP = model.PPP,
                DateUpdated = (model.DateUpdated == DateTime.MinValue) ? ("") : (model.DateUpdated.ToString("yyyy-MM-dd HH:mm:ss")),
            };
            if (channelViewModel.PPP == null) channelViewModel.PPP = "";

            int[] doors = new int[8];
            doors[0] = (int)(model.Door1 & 0xFFFF);
            doors[1] = (int)((model.Door1 >> 16) & 0xFFFF);
            doors[2] = (int)((model.Door1 >> 32) & 0xFFFF);
            doors[3] = (int)((model.Door1 >> 48) & 0xFFFF);
            doors[4] = (int)(model.Door2 & 0xFFFF);
            doors[5] = (int)((model.Door2 >> 16) & 0xFFFF);
            doors[6] = (int)((model.Door2 >> 32) & 0xFFFF);
            doors[7] = (int)((model.Door2 >> 48) & 0xFFFF);

            int doorCount = 8;
            for (int i = 7; i > 0; i--)
            {
                if (doors[i] == 0) doorCount--;
                else break;
            }

            channelViewModel.Doors = new bool[8][];
            channelViewModel.DoorVisibles = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                channelViewModel.Doors[i] = new bool[16];
                if (i < doorCount) channelViewModel.DoorVisibles[i] = true;
                else channelViewModel.DoorVisibles[i] = false;
                for (int j = 0; j < 16; j++)
                {
                    if ((doors[i] & (1 << j)) == 0) channelViewModel.Doors[i][j] = false;
                    else channelViewModel.Doors[i][j] = true;
                }
            }

            channelViewModel.Tags = new int[8];
            channelViewModel.Tags[0] = model.Tag1;
            channelViewModel.Tags[1] = model.Tag2;
            channelViewModel.Tags[2] = model.Tag3;
            channelViewModel.Tags[3] = model.Tag4;
            channelViewModel.Tags[4] = model.Tag5;
            channelViewModel.Tags[5] = model.Tag6;
            channelViewModel.Tags[6] = model.Tag7;
            channelViewModel.Tags[7] = model.Tag8;

            return channelViewModel;
        }

        private Channel GetChannel(ChannelViewModel model)
        {
            if (model == null) return null;

            Channel channel = new Channel()
            {
                ChannelIndex = model.ChannelIndex,
                Flat = model.Flat,
                PPP = model.PPP
            };

            if (channel.PPP == null) channel.PPP = "";

            int[] doors = new int[8];
            if (model.Doors != null)
            {
                for (int i = 0; i < model.Doors.Length; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        if (model.Doors[i][j]) doors[i] |= (1 << j);
                    }
                }
            }

            channel.Door1 = doors[3];
            channel.Door1 <<= 16;
            channel.Door1 |= (UInt32)doors[2];
            channel.Door1 <<= 16;
            channel.Door1 |= (UInt32)doors[1];
            channel.Door1 <<= 16;
            channel.Door1 |= (UInt32)doors[0];

            channel.Door2 = doors[7];
            channel.Door2 <<= 16;
            channel.Door2 |= (UInt32)doors[6];
            channel.Door2 <<= 16;
            channel.Door2 |= (UInt32)doors[5];
            channel.Door2 <<= 16;
            channel.Door2 |= (UInt32)doors[4];

            if (model.Tags != null)
            {
                channel.Tag1 = model.Tags[0];
                channel.Tag2 = model.Tags[1];
                channel.Tag3 = model.Tags[2];
                channel.Tag4 = model.Tags[3];
                channel.Tag5 = model.Tags[4];
                channel.Tag6 = model.Tags[5];
                channel.Tag7 = model.Tags[6];
                channel.Tag8 = model.Tags[7];
            }

            return channel;
        }

        private ScheduleViewModel GetScheduleViewModel(Schedule model)
        {
            if (model == null) return null;

            ScheduleViewModel scheduleViewModel = new ScheduleViewModel
            {
                ScheduleIndex = model.ScheduleIndex,
                Start1Hour = model.Start1Hour,
                Start1Minute = model.Start1Minute,
                End1Hour = model.End1Hour,
                End1Minute = model.End1Minute,
                Start2Hour = model.Start2Hour,
                Start2Minute = model.Start2Minute,
                End2Hour = model.End2Hour,
                End2Minute = model.End2Minute,
                DateUpdated = (model.DateUpdated == DateTime.MinValue) ? ("") : (model.DateUpdated.ToString("yyyy-MM-dd HH:mm:ss")),
            };
            scheduleViewModel.Day1 = new bool[8];
            for (int j = 0; j < 7; j++)
            {
                if ((model.Day1 & (1 << j)) == 0) scheduleViewModel.Day1[j] = false;
                else scheduleViewModel.Day1[j] = true;
            }
            scheduleViewModel.Day2 = new bool[8];
            for (int j = 0; j < 7; j++)
            {
                if ((model.Day2 & (1 << j)) == 0) scheduleViewModel.Day2[j] = false;
                else scheduleViewModel.Day2[j] = true;
            }

            return scheduleViewModel;
        }

        private Schedule GetSchedule(ScheduleViewModel model)
        {
            if (model == null) return null;

            Schedule schedule = new Schedule { ScheduleIndex = model.ScheduleIndex };

            schedule.Start1Hour = model.Start1Hour;
            schedule.Start1Minute = model.Start1Minute;
            schedule.End1Hour = model.End1Hour;
            schedule.End1Minute = model.End1Minute;

            schedule.Day1 = 0;
            if (model.Day1 != null)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (model.Day1[j]) schedule.Day1 |= (1 << j);
                }
            }

            schedule.Start2Hour = model.Start2Hour;
            schedule.Start2Minute = model.Start2Minute;
            schedule.End2Hour = model.End2Hour;
            schedule.End2Minute = model.End2Minute;

            schedule.Day2 = 0;
            if (model.Day2 != null)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (model.Day2[j]) schedule.Day2 |= (1 << j);
                }
            }

            return schedule;
        }

        private TradeCodeViewModel GetTradeCodeViewModel(TradeCode model)
        {
            if (model == null) return null;

            return new TradeCodeViewModel
            {
                TradeCodeIndex = model.TradeCodeIndex,
                PassNumber = model.PassNumber,
                ScheduleIndex = model.ScheduleIndex,
                DateUpdated = (model.DateUpdated == DateTime.MinValue) ? ("") : (model.DateUpdated.ToString("yyyy-MM-dd HH:mm:ss")),
            };
        }

        private TradeCode GetTradeCode(TradeCodeViewModel model)
        {
            if (model == null) return null;

            return new TradeCode
            {
                TradeCodeIndex = model.TradeCodeIndex,
                PassNumber = model.PassNumber,
                ScheduleIndex = model.ScheduleIndex,
            };
        }

        private DoorViewModel GetDoorViewModel(Door model)
        {
            if (model == null) return null;

            return new DoorViewModel
            {
                DoorIndex = model.DoorIndex,
                LockTimeout = model.LockTimeout,
                ScheduleIndex = model.ScheduleIndex,
                DateUpdated = (model.DateUpdated == DateTime.MinValue) ? ("") : (model.DateUpdated.ToString("yyyy-MM-dd HH:mm:ss")),
            };
        }

        private Door GetDoor(DoorViewModel model)
        {
            if (model == null) return null;

            return new Door
            {
                DoorIndex = model.DoorIndex,
                LockTimeout = model.LockTimeout,
                ScheduleIndex = model.ScheduleIndex,
            };
        }

        private StaffAccessViewModel GetStaffAccessViewModel(StaffAccess model)
        {
            if (model == null) return null;

            return new StaffAccessViewModel
            {
                StaffAccessIndex = model.StaffAccessIndex,
                AccessLevel = model.AccessLevel,
                PassNumber = model.PassNumber,
                DateUpdated = (model.DateUpdated == DateTime.MinValue) ? ("") : (model.DateUpdated.ToString("yyyy-MM-dd HH:mm:ss")),
            };
        }

        private StaffAccess GetStaffAccess(StaffAccessViewModel model)
        {
            if (model == null) return null;

            return new StaffAccess
            {
                StaffAccessIndex = model.StaffAccessIndex,
                AccessLevel = model.AccessLevel,
                PassNumber = model.PassNumber,
            };
        }

        private SystemOptionViewModel GetSystemOptionViewModel(SystemOption model)
        {
            if (model == null) return null;

            return new SystemOptionViewModel
            {
                Option1 = model.Option1,
                Option2 = model.Option2,
                TradeSchedule = model.TradeSchedule,
                RingTimeout = model.RingTimeout,
                AudioTimeout = model.AudioTimeout,
                WardenChannel = model.WardenChannel,
                CustomerNo = model.CustomerNo,
                SiteNo = model.SiteNo,
                DateUpdated = (model.DateUpdated == DateTime.MinValue) ? ("") : (model.DateUpdated.ToString("yyyy-MM-dd HH:mm:ss")),
            };
        }

        private SystemOption GetSystemOption(SystemOptionViewModel model)
        {
            if (model == null) return null;

            return new SystemOption
            {
                Option1 = model.Option1,
                Option2 = model.Option2,
                TradeSchedule = model.TradeSchedule,
                RingTimeout = model.RingTimeout,
                AudioTimeout = model.AudioTimeout,
                WardenChannel = model.WardenChannel,
                CustomerNo = model.CustomerNo,
                SiteNo = model.SiteNo,
            };
        }
   }
}