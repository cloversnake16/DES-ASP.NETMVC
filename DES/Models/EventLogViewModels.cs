using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DESCore.Models;

namespace DES.Models
{
    public class ServerEventLogViewModel
    {
        public IEnumerable<EventLogModel> Events { get; set; }
        public ServerEventLogQueryModel Query { get; set; }
    }

    public class LocalEventLogViewModel
    {
        public IEnumerable<EventLogModel> Events { get; set; }
        public LocalEventLogQueryModel Query { get; set; }
    }

    public class RemoteEventLogViewModel
    {
        public IEnumerable<RemoteSiteEventModel> Events { get; set; }
        public RemoteEventLogQueryModel Query { get; set; }
    }
}