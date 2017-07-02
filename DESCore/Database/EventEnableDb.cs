using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class EventEnableDb
    {
        public static string GetDescription(EventEnableModel model)
        {
            return "DoorOpenTag:" + model.DoorOpenTag + ", " +
                "DoorOpenHS:" + model.DoorOpenHS + ", " +
                "DoorOpenTrade:" + model.DoorOpenTrade + ", " +
                "DoorOpenExit:" + model.DoorOpenExit + ", " +
                "DoorOpenForsed:" + model.DoorOpenForsed + ", " +
                "DoorOpen:" + model.DoorOpen + ", " +
                "DoorOpenRemote:" + model.DoorOpenRemote + ", " +
                "DoorClosed:" + model.DoorClosed + ", " +
                "MainOn:" + model.MainOn + ", " +
                "MainOff:" + model.MainOff + ", " +
                "ProgChanged:" + model.ProgChanged;
        }
    }
}
