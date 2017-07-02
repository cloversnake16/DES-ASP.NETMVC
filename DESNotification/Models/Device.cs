using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESNotification.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string PhoneNumber { get; set; }
        public string IpAddress { get; set; }
        public string ACMVersion { get; set; }
        public long InBound { get; set; }
        public long OutBound { get; set; }
        public bool IsConnect { get; set; }
        public System.DateTime DateUpdated { get; set; }
    }
}
