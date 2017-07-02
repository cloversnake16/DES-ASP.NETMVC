using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public enum EventType { RetrieveFromSite, ProgramToSite, Email, OpenDoor, CreateSite, DeleteSite, UpdateSite, Login, Logout, LoadFromDb, SaveToDb };
    public enum EventStatus { Successful, Failure };

    public class EventLogModel
    {
        public string UserName { get; set; }
        public string SiteName { get; set; }
        public string DeviceId { get; set; }
        public EventType Event { get; set; }
        public EventStatus Status { get; set; }
        public EventLog EventLog { get; set; }
    }

    public enum ServerEventLogQueryType { UserName, SiteName, DeviceId, Event, Status };

    public class ServerEventLogQueryModel
    {
        public string[] QueryItems { get; set; }
        public string QueryKey { get; set; }
        public string QueryValue { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public UserModel UserModel { get; set; }
    }

    public enum LocalEventLogQueryType { Event, Status };
    
    public class LocalEventLogQueryModel
    {
        public string[] QueryItems { get; set; }
        public string QueryKey { get; set; }
        public string QueryValue { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public Json SiteModel { get; set; }
    }

    public class RemoteSiteEventModel
    {
        public string DeviceId { get; set; }
        public EventStatus Status { get; set; }
        public RemoteSiteEventLog RemoteSiteEventLog { get; set; }
    }

    public enum RemoteEventLogQueryType { EventDate, ACMDate, UpdateDate };

    public class RemoteEventLogQueryModel
    {
        public string[] QueryItems { get; set; }
        public string QueryKey { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public Json SiteModel { get; set; }
    }
}
