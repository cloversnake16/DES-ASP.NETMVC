using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public class EventEnableModel
    {
        public bool DoorOpenTag { get; set; }
        public bool DoorOpenHS { get; set; }
        public bool DoorOpenTrade { get; set; }
        public bool DoorOpenExit { get; set; }
        public bool DoorOpenForsed { get; set; }
        public bool DoorOpen { get; set; }
        public bool DoorOpenRemote { get; set; }
        public bool DoorClosed { get; set; }
        public bool MainOn { get; set; }
        public bool MainOff { get; set; }
        public bool ProgChanged { get; set; }
    }
}
