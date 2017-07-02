using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DESCore.Models;

namespace DES.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<Json> Sites { get; set; }
        public SiteQueryModel Query { get; set; }
    }

    public class ConnectiondViewModel
    {
        public Json Site { get; set; }
        public Device Device { get; set; }
    }

    public class SiteViewModel
    {
        public string SiteName { get; set; }
        public string DeviceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Error { get; set; }
    }
}