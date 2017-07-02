using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class EventLogDb
    {
        public IEnumerable<EventLogModel> GetServerEvents(ServerEventLogQueryModel query)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    List<EventLog> listEventLog = new List<EventLog>();
                    IEnumerable<User> users = db.Users.ToArray();
                    if (query.UserModel.IsAdmin)
                    {
                        if (!string.IsNullOrEmpty(query.QueryKey) && !string.IsNullOrEmpty(query.QueryValue))
                        {
                            ServerEventLogQueryType queryType = (ServerEventLogQueryType)Enum.Parse(typeof(ServerEventLogQueryType), query.QueryKey);
                            if (queryType == ServerEventLogQueryType.UserName)
                            {
                                users = users.Where(r => r.UserName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                            }
                        }
                    }
                    else users = users.Where(r => r.Id == query.UserModel.User.Id).ToArray();

                    foreach (var user in users)
                    {
                        var sites = user.Sites.ToArray();
                        if (!string.IsNullOrEmpty(query.QueryKey) && !string.IsNullOrEmpty(query.QueryValue))
                        {
                            ServerEventLogQueryType queryType = (ServerEventLogQueryType)Enum.Parse(typeof(ServerEventLogQueryType), query.QueryKey);
                            if (queryType == ServerEventLogQueryType.SiteName) sites = sites.Where(r => r.SiteName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                            else if (queryType == ServerEventLogQueryType.DeviceId) sites = sites.Where(r => r.DeviceId.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();

                            foreach (var site in sites)
                            {
                                var eventLogs = db.EventLogs.Where(r => r.SiteId == site.Id).ToArray();
                                if (query.FromDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateUpdated >= query.FromDateTime).ToArray();
                                if (query.ToDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateUpdated < query.ToDateTime + new TimeSpan(1, 0, 0, 0)).ToArray();
                                if (queryType == ServerEventLogQueryType.Event) eventLogs = eventLogs.Where(r => r.Event.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                                else if (queryType == ServerEventLogQueryType.Status) eventLogs = eventLogs.Where(r => r.Status.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();

                                listEventLog.AddRange(eventLogs);
                            }
                        }
                        else
                        {
                            var eventLogs = db.EventLogs.Where(r => r.UserId == user.Id).ToArray();
                            if (query.FromDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateUpdated >= query.FromDateTime).ToArray();
                            if (query.ToDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateUpdated < query.ToDateTime + new TimeSpan(1, 0, 0, 0)).ToArray();

                            listEventLog.AddRange(eventLogs);
                        }
                    }

                    return GetEventLogs(db, listEventLog.OrderByDescending(r => r.DateUpdated).ToArray());
                }
            }
            catch { }
            return null;
        }

        public IEnumerable<EventLogModel> GetLocalEventLogs(LocalEventLogQueryModel query)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    List<EventLog> listEventLog = new List<EventLog>();

                    var eventLogs = db.EventLogs.Where(r => r.SiteId == query.SiteModel.Site.Id).ToArray();
                    if (query.FromDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateUpdated >= query.FromDateTime).ToArray();
                    if (query.ToDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateUpdated < query.ToDateTime + new TimeSpan(1, 0, 0, 0)).ToArray();

                    if (!string.IsNullOrEmpty(query.QueryKey) && !string.IsNullOrEmpty(query.QueryValue))
                    {
                        LocalEventLogQueryType queryType = (LocalEventLogQueryType)Enum.Parse(typeof(LocalEventLogQueryType), query.QueryKey);
                        if (queryType == LocalEventLogQueryType.Event) eventLogs = eventLogs.Where(r => r.Event.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                        else if (queryType == LocalEventLogQueryType.Status) eventLogs = eventLogs.Where(r => r.Status.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                    }

                    return GetEventLogs(db, eventLogs.OrderByDescending(r => r.DateUpdated).ToArray());
                }
            }
            catch { }
            return null;
        }

        private IEnumerable<EventLogModel> GetEventLogs(DESEntities db, EventLog[] eventLogs)
        {
            try
            {
                List<EventLogModel> listEventLog = new List<EventLogModel>();
                if (eventLogs != null)
                {
                    foreach (var log in eventLogs)
                    {
                        var user = db.Users.Where(r => r.Id == log.UserId).FirstOrDefault();
                        string userName = "";
                        if (user != null) userName = user.UserName;

                        var site = db.Sites.Where(r => r.Id == log.SiteId).FirstOrDefault();
                        string siteName = "";
                        if (site != null) siteName = site.SiteName;

                        listEventLog.Add(new EventLogModel
                        {
                            UserName = userName,
                            SiteName = siteName,
                            EventLog = log,
                        });
                    }
                }
                return listEventLog;
            }
            catch { }
            return null;
        }

        public EventLog GetEventLog(int eventId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.EventLogs.Where(r => r.Id == eventId).FirstOrDefault();
                }
            }
            catch { }
            return null;
        }

        public static void LogEvent(EventLogModel model)
        {
            try
            {
                if (model == null) return;
                using (var db = new DESEntities())
                {
                    if (string.IsNullOrEmpty(model.DeviceId)) return;
                    Site site = db.Sites.Where(r => r.DeviceId == model.DeviceId).FirstOrDefault();
                    if (site == null) return;

                    EventLog eventlog = new EventLog
                    {
                        UserId = site.UserId,
                        SiteId = site.Id,
                        Event = model.Event.ToString(),
                        Status = model.Status.ToString(),
                        Description = model.EventLog.Description,
                        Request = model.EventLog.Request,
                        Response = model.EventLog.Response,
                        DateUpdated = DateTime.Now
                    };
                    db.EventLogs.Add(eventlog);
                    db.SaveChanges();
                }
            }
            catch (Exception ex) { string err = ex.Message; }
        }

        public void SaveEvent(EventLog eventlog)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    eventlog.DateUpdated = DateTime.Now;
                    db.EventLogs.Add(eventlog);
                    db.SaveChanges();
                }
            }
            catch (Exception ex) { string err = ex.Message; }
        }

        public bool Delete(int siteId, int maxSize)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    int count = db.EventLogs.Where(r => r.SiteId == siteId).Count() - maxSize;
                    if(count > 0)
                    {
                        var eventLogs = db.EventLogs.OrderBy(r => r.DateUpdated).Skip(0).Take(count);
                        if (eventLogs != null || eventLogs.Count() > 0)
                        {
                            db.EventLogs.RemoveRange(eventLogs);
                            db.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception Exception) { string err = Exception.Message; }
            return false;
        }

        public IEnumerable<RemoteSiteEventModel> GetRemoteEvents(RemoteEventLogQueryModel query)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var eventLogs = db.RemoteSiteEventLogs.Where(r => r.SiteId == query.SiteModel.Site.Id).ToArray();

                    if (!string.IsNullOrEmpty(query.QueryKey))
                    {
                        RemoteEventLogQueryType queryType = (RemoteEventLogQueryType)Enum.Parse(typeof(RemoteEventLogQueryType), query.QueryKey);
                        if (queryType == RemoteEventLogQueryType.EventDate)
                        {
                            if (query.FromDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateEvent >= query.FromDateTime).ToArray();
                            if (query.ToDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateEvent < query.ToDateTime + new TimeSpan(1, 0, 0, 0)).ToArray();
                        }
                        else if (queryType == RemoteEventLogQueryType.ACMDate)
                        {
                            if (query.FromDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateACM >= query.FromDateTime).ToArray();
                            if (query.ToDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateACM < query.ToDateTime + new TimeSpan(1, 0, 0, 0)).ToArray();
                        }
                        else if (queryType == RemoteEventLogQueryType.UpdateDate)
                        {
                            if (query.FromDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateUpdated >= query.FromDateTime).ToArray();
                            if (query.ToDateTime != DateTime.MinValue) eventLogs = eventLogs.Where(r => r.DateUpdated < query.ToDateTime + new TimeSpan(1, 0, 0, 0)).ToArray();
                        }
                    }

                    eventLogs = eventLogs.OrderByDescending(r => r.DateUpdated).ToArray();
                    List<RemoteSiteEventModel> listEventLog = new List<RemoteSiteEventModel>();
                    if (eventLogs != null)
                    {
                        foreach (var log in eventLogs)
                        {
                            listEventLog.Add(new RemoteSiteEventModel
                            {
                                RemoteSiteEventLog = log,
                            });
                        }
                    }
                    return listEventLog;
                }
            }
            catch { }
            return null;
        }

        public static void SaveRemoteEvent(List<RemoteSiteEventModel> listModel)
        {
            try
            {
                if (listModel.Count == 0) return;

                using (var db = new DESEntities())
                {
                    Site site = db.Sites.Where(r => r.DeviceId == listModel[0].DeviceId).FirstOrDefault();
                    if (site == null) return;

                    List<RemoteSiteEventLog> logs = new List<Models.RemoteSiteEventLog>();
                    foreach (var model in listModel)
                    {
                        logs.Add(new RemoteSiteEventLog
                        {
                            UserId = site.UserId,
                            SiteId = site.Id,
                            Status = model.Status.ToString(),
                            EventNumber = model.RemoteSiteEventLog.EventNumber,
                            DateEvent = model.RemoteSiteEventLog.DateEvent,
                            DateACM = model.RemoteSiteEventLog.DateACM,
                            Description = model.RemoteSiteEventLog.Description,
                            Request = model.RemoteSiteEventLog.Request,
                            Response = model.RemoteSiteEventLog.Response,
                            DateUpdated = DateTime.Now,
                        });
                    }

                    if (logs.Count > 0)
                    {
                        db.RemoteSiteEventLogs.AddRange(logs);
                        db.SaveChanges();
                    }
                }
            }
            catch { }
        }
    }
}
