using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DES.Models
{
    public class ReservedChannels
    {
        public bool[] Channels { get; set; }
    }

    public class SetViewModel
    {
        public int CheckPeriod { get; set; }
        public int MaxDataSize { get; set; }
        public int MaxEventlogSize { get; set; }
        public int MonthReset { get; set; }
        public int DayReset { get; set; }
        public int ReservedChannels { get; set; }
        public string[] DoorDescriptions { get; set; }
    }
}