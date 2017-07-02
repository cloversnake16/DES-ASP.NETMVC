using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public class Json
    {
        public Site Site { get; set; }
        public string UserName { get; set; }
        public string ACMVersion { get; set; }
        public double Version { get; set; }
        public bool IsConnect { get; set; }
    }

    public enum SiteQueryType { UserName, SiteName, DeviceId, CustomerName, CustomerAddress, CustomerPhone };
    public class SiteQueryModel
    {
        public string[] QueryItems { get; set; }
        public string QueryKey { get; set; }
        public string QueryValue { get; set; }
        public UserModel UserModel { get; set; }
    }
}
