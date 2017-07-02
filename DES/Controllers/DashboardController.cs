using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DESCore.Models;
using DESCore.Database;
using DES.Models;
using DESCore.Helpers;
using DESCore.DesCommands;
using System.Web.Helpers;

namespace DES.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                Session["SiteModel"] = null;

                if (Session["UserModel"] == null) return RedirectToAction("Index", "Account");
                UserModel userModel = (UserModel)Session["UserModel"];

                SiteQueryModel query = new SiteQueryModel
                {
                    QueryItems = Enum.GetNames(typeof(SiteQueryType)),
                    UserModel = userModel,
                };

                DashboardViewModel model = new DashboardViewModel
                {
                    Sites = new SiteDb().GetSites(query),
                    Query = query,
                };

                Session["DashboardViewModel"] = model;
                return View(model);
            }
            catch { return RedirectToAction("Logout", "Account"); }            
        }

        public ActionResult ConnectionStatus()
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            DashboardViewModel dashboard = (DashboardViewModel)Session["DashboardViewModel"];

            List<ConnectiondViewModel> model = new List<ConnectiondViewModel>();
            if (dashboard.Sites != null && dashboard.Sites.Count() > 0)
            {
                foreach (DESCore.Models.Json site in dashboard.Sites)
                {                    
                    var device = DeviceDb.GetDevice(site.Site.DeviceId);
                    if (device != null)
                    {
                        model.Add(new ConnectiondViewModel
                        {
                            Site = site,
                            Device = device
                        });
                    }
                }
            }

            return View(model);
        }

        public ActionResult Search(SiteQueryModel query)
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            DashboardViewModel dashboardViewModel = (DashboardViewModel)Session["DashboardViewModel"];

            dashboardViewModel.Query.QueryKey = query.QueryKey;
            dashboardViewModel.Query.QueryValue = query.QueryValue;
            dashboardViewModel.Sites = new SiteDb().GetSites(dashboardViewModel.Query);

            return RedirectToAction("Index");
        }

        public ActionResult ClearQuery()
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            DashboardViewModel dashboardViewModel = (DashboardViewModel)Session["DashboardViewModel"];

            dashboardViewModel.Query.QueryKey = null;
            dashboardViewModel.Query.QueryValue = null;

            return RedirectToAction("Index");
        }

        public ActionResult SearchEventLog(ServerEventLogQueryModel query)
        {
            if (Session["ServerEventLogViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            ServerEventLogViewModel eventLogViewModel = (ServerEventLogViewModel)Session["ServerEventLogViewModel"];

            eventLogViewModel.Query.QueryKey = query.QueryKey;
            eventLogViewModel.Query.QueryValue = query.QueryValue;
            eventLogViewModel.Query.FromDateTime = query.FromDateTime;
            eventLogViewModel.Query.ToDateTime = query.ToDateTime;
            eventLogViewModel.Events = new EventLogDb().GetServerEvents(eventLogViewModel.Query);

            return RedirectToAction("EventLog");
        }

        public ActionResult ClearEventLog()
        {
            if (Session["ServerEventLogViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            ServerEventLogViewModel eventLogViewModel = (ServerEventLogViewModel)Session["ServerEventLogViewModel"];

            eventLogViewModel.Query.QueryKey = null;
            eventLogViewModel.Query.QueryValue = null;
            eventLogViewModel.Query.FromDateTime = DateTime.Today;
            eventLogViewModel.Query.ToDateTime = DateTime.Today;

            return RedirectToAction("EventLog");
        }

        public ActionResult EventLog()
        {
            if (Session["UserModel"] == null) return RedirectToAction("Index", "Account");
            UserModel userModel = (UserModel)Session["UserModel"];

            ServerEventLogQueryModel query = new ServerEventLogQueryModel
            {
                QueryItems = Enum.GetNames(typeof(ServerEventLogQueryType)),
                QueryKey = null,
                QueryValue = null,
                FromDateTime = DateTime.Today,
                ToDateTime = DateTime.Today,
                UserModel = userModel,
            };

            ServerEventLogViewModel model = new ServerEventLogViewModel
            {
                Events = new EventLogDb().GetServerEvents(query),
                Query = query,
            };

            return View(model);
        }

        public ActionResult DetailedEventLog(int EventId)
        {
            if (Session["ServerEventLogViewModel"] == null) return RedirectToAction("LocalEventLog");
            ServerEventLogViewModel serverEventLogViewModel = (ServerEventLogViewModel)Session["ServerEventLogViewModel"];

            EventLogModel eventLog = serverEventLogViewModel.Events.Where(r => r.EventLog.Id == EventId).FirstOrDefault();
            return View(eventLog);            
        }

        public ActionResult Resident(int SiteId)
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            DashboardViewModel dashboard = (DashboardViewModel)Session["DashboardViewModel"];

            Session["SiteModel"] = dashboard.Sites.Where(r => r.Site.Id == SiteId).FirstOrDefault();
            Session["ResidentsViewModel"] = null;
            return RedirectToAction("Index", "Resident");
        }

        public ActionResult OpenDoor(int SiteId)
        {
            try
            {
                DashboardViewModel dashboard = (DashboardViewModel)Session["DashboardViewModel"];

                DESCore.Models.Json siteModel = dashboard.Sites.Where(r => r.Site.Id == SiteId).FirstOrDefault();
                Session["SiteModel"] = siteModel;

                string[] model = new string[128];
                var doorDescriptions = SettingDb.GetDoorDescriptions();
                if (doorDescriptions != null)
                {
                    foreach (var doorDescription in doorDescriptions)
                    {
                        model[doorDescription.Index] = doorDescription.Contents;
                    }
                }
                return View(model);
            }
            catch { return RedirectToAction("Index"); }            
        }

        public JsonResult OpenDoorCommand(int Index)
        {
            try
            {
                DESCore.Models.Json siteModel = (DESCore.Models.Json)Session["SiteModel"];
                string deviceId = siteModel.Site.DeviceId;
                return Json(CommandHelper.OpenDoor(deviceId, Index - 1));
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message }); }
        }

        public ActionResult MoveSite(int SiteId)
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index", "Dashboard");
            DashboardViewModel dashboard = (DashboardViewModel)Session["DashboardViewModel"];

            Session["SiteModel"] = dashboard.Sites.Where(r => r.Site.Id == SiteId).FirstOrDefault();
            return RedirectToAction("Index", "Site");
        }

        public ActionResult Create()
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index");
            DashboardViewModel dashboardViewModel = (DashboardViewModel)Session["DashboardViewModel"];

            return View(new SiteViewModel());
        }

        [HttpPost]
        public ActionResult Create(SiteViewModel model)
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index");
            DashboardViewModel dashboardViewModel = (DashboardViewModel)Session["DashboardViewModel"];

            if (string.IsNullOrEmpty(model.SiteName))
            {
                model.Error = "Required SiteName.";
                return View(model);
            }
            if (string.IsNullOrEmpty(model.DeviceId))
            {
                model.Error = "Required DeviceId.";
                return View(model);
            }

            var site = new Site
            {
                SiteName = model.SiteName,
                UserId = dashboardViewModel.Query.UserModel.User.Id,
                DeviceId = model.DeviceId,
                CustomerName = model.CustomerName,
                CustomerAddress = model.CustomerAddress,
                CustomerPhone = model.CustomerPhone,
                Address2 = model.Address2,
                Area = model.Area,
                Town = model.Town,
                City = model.City,
                Country = model.Country,
            };

            ResultModel result = new SiteDb().CreateSite(site);
            if (result.Status)
            {
                site = (Site)result.Result;
                new EventLogDb().SaveEvent(new EventLog
                {
                    UserId = dashboardViewModel.Query.UserModel.User.Id,
                    SiteId = site.Id,
                    Event = EventType.CreateSite.ToString(),
                    Status = EventStatus.Successful.ToString(),
                    Description = site.SiteName + " was created.",
                });

                Session["DashboardViewModel"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                site = (Site)result.Result;
                new EventLogDb().SaveEvent(new EventLog
                {
                    UserId = dashboardViewModel.Query.UserModel.User.Id,
                    SiteId = 0,
                    Event = EventType.CreateSite.ToString(),
                    Status = EventStatus.Failure.ToString(),
                    Description = (string)result.Result,
                });
            }

            model.Error = (string)result.Result;
            return View(model);
        }

        public ActionResult Update(int SiteId)
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index");
            DashboardViewModel dashboardViewModel = (DashboardViewModel)Session["DashboardViewModel"];

            DESCore.Models.Json siteModel = ((DashboardViewModel)Session["DashboardViewModel"]).Sites.Where(r => r.Site.Id == SiteId).FirstOrDefault();
            Session["SiteModel"] = siteModel;
            return View(new SiteViewModel
            {
                SiteName = siteModel.Site.SiteName,
                DeviceId = siteModel.Site.DeviceId,
                CustomerName = siteModel.Site.CustomerName,
                CustomerAddress = siteModel.Site.CustomerAddress,
                CustomerPhone = siteModel.Site.CustomerPhone,
                Address2 = siteModel.Site.Address2,
                Area = siteModel.Site.Area,
                Town = siteModel.Site.Town,
                City = siteModel.Site.City,
                Country = siteModel.Site.Country,
            });
        }
        
        [HttpPost]
        public ActionResult Update(SiteViewModel model)
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index");
            DashboardViewModel dashboardViewModel = (DashboardViewModel)Session["DashboardViewModel"];

            DESCore.Models.Json siteModel = ((DESCore.Models.Json)Session["SiteModel"]);

            model.SiteName = siteModel.Site.SiteName;

            if (string.IsNullOrEmpty(model.DeviceId))
            {
                model.Error = "Required DeviceId.";
                return View(model);
            }

            var site = new Site
            {
                Id = siteModel.Site.Id,
                DeviceId = model.DeviceId,
                CustomerName = model.CustomerName,
                CustomerAddress = model.CustomerAddress,
                CustomerPhone = model.CustomerPhone,
                Address2 = model.Address2,
                Area = model.Area,
                Town = model.Town,
                City = model.City,
                Country = model.Country,
            };

            ResultModel result = new SiteDb().UpdateSite(site);
            if (result.Status)
            {
                new EventLogDb().SaveEvent(new EventLog
                {
                    UserId = dashboardViewModel.Query.UserModel.User.Id,
                    SiteId = site.Id,
                    Event = EventType.UpdateSite.ToString(),
                    Status = EventStatus.Successful.ToString(),
                    Description = model.SiteName + " was updated.",
                });
                Session["DashboardViewModel"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                new EventLogDb().SaveEvent(new EventLog
                {
                    UserId = dashboardViewModel.Query.UserModel.User.Id,
                    SiteId = site.Id,
                    Event = EventType.UpdateSite.ToString(),
                    Status = EventStatus.Failure.ToString(),
                    Description = (string)result.Result,
                });
            }

            model.Error = (string)result.Result;
            return View(model);
        }

        public ActionResult Delete(int SiteId)
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index");
            DashboardViewModel dashboardViewModel = (DashboardViewModel)Session["DashboardViewModel"];

            DESCore.Models.Json siteModel = ((DashboardViewModel)Session["DashboardViewModel"]).Sites.Where(r => r.Site.Id == SiteId).FirstOrDefault();
            Session["SiteModel"] = siteModel;
            return View(new SiteViewModel
            {
                SiteName = siteModel.Site.SiteName,
                DeviceId = siteModel.Site.DeviceId,
                CustomerName = siteModel.Site.CustomerName,
                CustomerAddress = siteModel.Site.CustomerAddress,
                CustomerPhone = siteModel.Site.CustomerPhone,
                Address2 = siteModel.Site.Address2,
                Area = siteModel.Site.Area,
                Town = siteModel.Site.Town,
                City = siteModel.Site.City,
                Country = siteModel.Site.Country,
            });
        }

        [HttpPost]
        public ActionResult Delete(SiteViewModel model)
        {
            if (Session["DashboardViewModel"] == null) return RedirectToAction("Index");
            DashboardViewModel dashboardViewModel = (DashboardViewModel)Session["DashboardViewModel"];

            DESCore.Models.Json siteModel = ((DESCore.Models.Json)Session["SiteModel"]);

            model.SiteName = siteModel.Site.SiteName;
            model.DeviceId = siteModel.Site.DeviceId;
            model.CustomerName = siteModel.Site.CustomerName;
            model.CustomerAddress = siteModel.Site.CustomerAddress;
            model.CustomerPhone = siteModel.Site.CustomerPhone;
            model.Address2 = siteModel.Site.Address2;
            model.Area = siteModel.Site.Area;
            model.Town = siteModel.Site.Town;
            model.City = siteModel.Site.City;
            model.Country = siteModel.Site.Country;

            var site = new Site
            {
                Id = siteModel.Site.Id,
            };

            ResultModel result = new SiteDb().DeleteSite(site);
            if (result.Status)
            {
                new EventLogDb().SaveEvent(new EventLog
                {
                    UserId = dashboardViewModel.Query.UserModel.User.Id,
                    SiteId = site.Id,
                    Event = EventType.DeleteSite.ToString(),
                    Status = EventStatus.Successful.ToString(),
                    Description = model.SiteName + " was deleted.",
                });
                Session["DashboardViewModel"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                new EventLogDb().SaveEvent(new EventLog
                {
                    UserId = dashboardViewModel.Query.UserModel.User.Id,
                    SiteId = site.Id,
                    Event = EventType.DeleteSite.ToString(),
                    Status = EventStatus.Failure.ToString(),
                    Description = (string)result.Result,
                });
            }

            model.Error = (string)result.Result;
            return View(model);
        }

        [HttpPost]
        public ActionResult GetCurrentConnectionStatus()
        {
            try
            {
                Session["SiteModel"] = null;

                if (Session["UserModel"] == null) return null;
                UserModel userModel = (UserModel)Session["UserModel"];

                SiteQueryModel query = new SiteQueryModel
                {
                    QueryItems = Enum.GetNames(typeof(SiteQueryType)),
                    UserModel = userModel,
                };

                DashboardViewModel model = new DashboardViewModel
                {
                    Sites = new SiteDb().GetSites(query),
                    Query = query,
                };

                Session["DashboardViewModel"] = model;
                List<string> siteInfo = new List<string>();
                for (int i=0; i<model.Sites.Count(); ++i)
                {
                    DESCore.Models.Json eachSite = model.Sites.ElementAt(i);
                    siteInfo.Add(eachSite.Site.DeviceId+","+Convert.ToString(eachSite.IsConnect)+","+eachSite.ACMVersion);
                }
                return new JsonResult{ Data = siteInfo };                
            }
            catch { return null; }
        }
    }
}