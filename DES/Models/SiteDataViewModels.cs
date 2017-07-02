using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DESCore.Models;

namespace DES.Models
{
    public class SiteDataViewModel
    {
        public ChannelViewModel[] ChannelViewModels { get; set; }
        public ScheduleViewModel[] ScheduleViewModels { get; set; }
        public TradeCodeViewModel[] TradeCodeViewModels { get; set; }
        public DoorViewModel[] DoorViewModels { get; set; }
        public StaffAccessViewModel[] StaffAccessViewModels { get; set; }
        public SystemOptionViewModel SystemOptionViewModel { get; set; }
        public bool IsNew { get; set; }
        public string[] DoorDescriptions { get; set; }
    }

    public class SiteParaViewModel
    {
        public SiteData SiteData { get; set; }
        public SiteDataType SiteDataType { get; set; }
        public SiteParaType SiteParaType { get; set; }
        public bool All { get; set; }
        public bool Next { get; set; }
        public ChannelViewModel ChannelViewModel { get; set; }
        public ScheduleViewModel ScheduleViewModel { get; set; }
        public TradeCodeViewModel TradeCodeViewModel { get; set; }
        public DoorViewModel DoorViewModel { get; set; }
        public StaffAccessViewModel StaffAccessViewModel { get; set; }
        public SystemOptionViewModel SystemOptionViewModel { get; set; }
    }

    public class ChannelViewModel
    {
        public int ChannelIndex { get; set; }
        public int Flat { get; set; }
        public string PPP { get; set; }
        public bool[][] Doors { get; set; }
        public bool[] DoorVisibles { get; set; }
        public int[] Tags { get; set; }
        public string DateUpdated { get; set; }
        public bool Selected { get; set; }
    }

    public class ScheduleViewModel
    {
        public int ScheduleIndex { get; set; }
        public int Start1Hour { get; set; }
        public int Start1Minute { get; set; }
        public int End1Hour { get; set; }
        public int End1Minute { get; set; }
        public bool[] Day1 { get; set; }
        public int Start2Hour { get; set; }
        public int Start2Minute { get; set; }
        public int End2Hour { get; set; }
        public int End2Minute { get; set; }
        public bool[] Day2 { get; set; }
        public string DateUpdated { get; set; }
    }

    public class TradeCodeViewModel
    {
        public int TradeCodeIndex { get; set; }
        public int PassNumber { get; set; }
        public int ScheduleIndex { get; set; }
        public string DateUpdated { get; set; }
    }

    public partial class DoorViewModel
    {
        public int DoorIndex { get; set; }
        public int LockTimeout { get; set; }
        public int ScheduleIndex { get; set; }
        public string DateUpdated { get; set; }
    }

    public partial class StaffAccessViewModel
    {
        public int StaffAccessIndex { get; set; }
        public int AccessLevel { get; set; }
        public int PassNumber { get; set; }
        public string DateUpdated { get; set; }
    }

    public partial class SystemOptionViewModel
    {
        public int Option1 { get; set; }
        public int Option2 { get; set; }
        public int TradeSchedule { get; set; }
        public int RingTimeout { get; set; }
        public int AudioTimeout { get; set; }
        public int WardenChannel { get; set; }
        public int CustomerNo { get; set; }
        public int SiteNo { get; set; }
        public string DateUpdated { get; set; }
    }
}