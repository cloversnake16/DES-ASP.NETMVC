using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.General;
using DESCore.Database;
using DESCore.Models;

namespace DESCore.DesCommands
{
    public class ProgramCommand
    {
        public static byte[] ProgramChannel(Channel channel)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.CHANNEL);
            data.AddRange(Encoding.Default.GetBytes((channel.ChannelIndex - 1).ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes(channel.Flat.ToString("D4")));
            short door = (short)(channel.Door1 & 0xFFFF);
            data.AddRange(Encoding.Default.GetBytes(door.ToString("X4")));
            data.AddRange(Encoding.Default.GetBytes(channel.PPP.Replace("0", "A")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramChannelDoor(int index, int groupIndex, int door)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.CHANNEL_DOOR);
            data.AddRange(Encoding.Default.GetBytes((index - 1).ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes(groupIndex.ToString()));
            data.AddRange(Encoding.Default.GetBytes(door.ToString("X4")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramTag(int index, int tagIndex, int tag)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.CHANNEL_TAG);
            data.AddRange(Encoding.Default.GetBytes((index - 1).ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes((tagIndex + 1).ToString()));
            data.AddRange(Encoding.Default.GetBytes(tag.ToString("X8")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramSchedule(Schedule schedule)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.SCHEDULE);
            data.AddRange(Encoding.Default.GetBytes(schedule.ScheduleIndex.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.Start1Hour.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.Start1Minute.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.End1Hour.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.End1Minute.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.Day1.ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.Start2Hour.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.Start2Minute.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.End2Hour.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.End2Minute.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(schedule.Day2.ToString("X2")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramTradeCode(TradeCode tradeCode)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.TRADECODE);
            data.AddRange(Encoding.Default.GetBytes(tradeCode.TradeCodeIndex.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(tradeCode.PassNumber.ToString("D6")));
            data.AddRange(Encoding.Default.GetBytes(tradeCode.ScheduleIndex.ToString("D2")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramDoor(Door door)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.DOOR);
            data.AddRange(Encoding.Default.GetBytes(door.DoorIndex.ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes(door.LockTimeout.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(door.ScheduleIndex.ToString("D2")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramStaffAccess(StaffAccess staffAccess)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.STAFFACCESS);
            data.AddRange(Encoding.Default.GetBytes(staffAccess.StaffAccessIndex.ToString("D2")));
            data.Add((byte)staffAccess.AccessLevel);
            data.AddRange(Encoding.Default.GetBytes(staffAccess.PassNumber.ToString("D6")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramSystemOptions(SystemOption systemOptions)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.SYSTEMOPTIONS);
            data.AddRange(Encoding.Default.GetBytes(systemOptions.Option1.ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes(systemOptions.TradeSchedule.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(systemOptions.RingTimeout.ToString("D3")));
            data.AddRange(Encoding.Default.GetBytes(systemOptions.AudioTimeout.ToString("D3")));
            data.AddRange(Encoding.Default.GetBytes(systemOptions.WardenChannel.ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes(systemOptions.CustomerNo.ToString("D4")));
            data.AddRange(Encoding.Default.GetBytes(systemOptions.SiteNo.ToString("D4")));
            data.AddRange(Encoding.Default.GetBytes(systemOptions.Option2.ToString("X2")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramDateTime(DateTime dateTime)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.DATETIME);
            data.AddRange(Encoding.Default.GetBytes(dateTime.Year.ToString("D4")));
            data.AddRange(Encoding.Default.GetBytes(dateTime.Month.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(dateTime.Day.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(dateTime.Hour.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(dateTime.Minute.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(dateTime.Second.ToString("D2")));
            data.AddRange(Encoding.Default.GetBytes(((int)dateTime.DayOfWeek).ToString("D1")));
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] ProgramEventEnable(EventEnableModel model)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.EVENT_ENABLE);
            data.AddRange(new byte[] { (byte)'1', (model.DoorOpenTag ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'2', (model.DoorOpenHS ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'3', (model.DoorOpenTrade ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'4', (model.DoorOpenExit ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'5', (model.DoorOpenForsed ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'6', (model.DoorOpen ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'7', (model.DoorOpenRemote ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'D', (model.DoorClosed ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'M', (model.MainOn ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'m', (model.MainOff ? ((byte)'1') : ((byte)'0')) });
            data.AddRange(new byte[] { (byte)'p', (model.ProgChanged ? ((byte)'1') : ((byte)'0')) });
            return DesCommand.GetCommand(Constants.PROGRAM, data.ToArray());
        }

        public static byte[] OpenDoor(int index)
        {
            List<byte> data = new List<byte>();
            data.AddRange(Encoding.Default.GetBytes(index.ToString("D2")));
            return DesCommand.GetCommand(Constants.DOOR, data.ToArray());
        }
    }
}
