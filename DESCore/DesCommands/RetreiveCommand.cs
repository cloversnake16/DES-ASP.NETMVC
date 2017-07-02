using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using DESCore.General;
using DESCore.Models;

namespace DESCore.DesCommands
{
    public class RetreiveCommand
    {
        public static byte[] RetreiveACMVersion()
        {
            return DesCommand.GetCommand(Constants.VERSION);
        }

        public static string RetreiveACMVersion(byte[] data)
        {
            string version = "";
            int offset = 15;
            if (data[17] != '.') offset++;

            version = Encoding.UTF8.GetString(data, offset, 5).Trim();
            offset += 5;

            if (data[offset] == 'b')
            {
                offset++;
                version += " beta " + Encoding.UTF8.GetString(data, offset, 2).Trim();
            }

            return version;
        }

        public static byte[] RetreiveChannel(int index)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.CHANNEL);
            data.AddRange(Encoding.Default.GetBytes((index - 1).ToString("X2")));
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static Channel RetreiveChannel(byte[] data)
        {
            Channel channel = new Channel();

            int offset = 16;
            channel.ChannelIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16) + 1;
            offset += 2;
            string flat = Encoding.Default.GetString(data, offset, 4);
            channel.Flat = Convert.ToInt32(flat);
            offset += 4;
            string door = Encoding.Default.GetString(data, offset, 4);
            channel.Door1 = Convert.ToInt32(door, 16);
            offset += 4;
            int len = data.Length - offset - 3;
            channel.PPP = Encoding.Default.GetString(data, offset, len).Replace("A", "0");
            long ppp;
            if (!long.TryParse(channel.PPP, out ppp)) channel.PPP = "0";

            return channel;
        }

        public static byte[] RetreiveChannelDoor(int index, int groupIndex)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.CHANNEL_DOOR);
            data.AddRange(Encoding.Default.GetBytes((index - 1).ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes(groupIndex.ToString()));
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static int RetreiveChannelDoor(byte[] data)
        {
            int offset = 16;
            int channelIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16) + 1;
            offset += 2;
            int groupIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 1));
            offset++;
            return Convert.ToInt32(Encoding.Default.GetString(data, 19, 4), 16);
        }

        public static byte[] RetreiveTag(int index, int tagIndex)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.CHANNEL_TAG);
            data.AddRange(Encoding.Default.GetBytes((index - 1).ToString("X2")));
            data.AddRange(Encoding.Default.GetBytes((tagIndex + 1).ToString()));
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static int RetreiveTag(byte[] data)
        {
            int offset = 16;
            int channelIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16) + 1;
            offset += 2;
            int tagIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 1));
            offset++;
            return Convert.ToInt32(Encoding.Default.GetString(data, 19, 8), 16);
        }

        public static byte[] RetreiveSchedule(int index)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.SCHEDULE);
            data.AddRange(Encoding.Default.GetBytes(index.ToString("D2")));
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static Schedule RetreiveSchedule(byte[] data)
        {
            Schedule schedule = new Schedule();

            int offset = 16;
            schedule.ScheduleIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.Start1Hour = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.Start1Minute = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.End1Hour = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.End1Minute = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.Day1 = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16);
            offset += 2;
            schedule.Start2Hour = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.Start2Minute = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.End2Hour = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.End2Minute = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            schedule.Day2 = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16);
            offset += 2;

            return schedule;
        }

        public static byte[] RetreiveTradeCode(int index)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.TRADECODE);
            data.AddRange(Encoding.Default.GetBytes(index.ToString("D2")));
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static TradeCode RetreiveTradeCode(byte[] data)
        {
            TradeCode tradeCode = new TradeCode();

            int offset = 16;
            tradeCode.TradeCodeIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            tradeCode.PassNumber = Convert.ToInt32(Encoding.Default.GetString(data, offset, 6));
            offset += 6;
            tradeCode.ScheduleIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));

            return tradeCode;
        }

        public static byte[] RetreiveDoor(int index)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.DOOR);
            data.AddRange(Encoding.Default.GetBytes(index.ToString("X2")));
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static Door RetreiveDoor(byte[] data)
        {
            Door door = new Door();

            int offset = 16;
            door.DoorIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16);
            offset += 2;
            door.LockTimeout = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            door.ScheduleIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));

            return door;
        }

        public static byte[] RetreiveStaffAccess(int index)
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.STAFFACCESS);
            data.AddRange(Encoding.Default.GetBytes(index.ToString("D2")));
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static StaffAccess RetreiveStaffAccess(byte[] data)
        {
            StaffAccess staffAccess = new StaffAccess();

            int offset = 16;
            staffAccess.StaffAccessIndex = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            staffAccess.AccessLevel = (data[offset] & 0xF);
            offset++;
            staffAccess.PassNumber = Convert.ToInt32(Encoding.Default.GetString(data, offset, 6));

            return staffAccess;
        }

        public static byte[] RetreiveSystemOptions()
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.SYSTEMOPTIONS);
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static SystemOption RetreiveSystemOptions(byte[] data)
        {
            SystemOption systemOptions = new SystemOption();

            int offset = 16;
            systemOptions.Option1 = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16);
            offset += 2;
            systemOptions.TradeSchedule = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            systemOptions.RingTimeout = Convert.ToInt32(Encoding.Default.GetString(data, offset, 3));
            offset += 3;
            systemOptions.AudioTimeout = Convert.ToInt32(Encoding.Default.GetString(data, offset, 3));
            offset += 3;
            systemOptions.WardenChannel = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16);
            offset += 2;
            systemOptions.CustomerNo = Convert.ToInt32(Encoding.Default.GetString(data, offset, 4));
            offset += 4;
            systemOptions.SiteNo = Convert.ToInt32(Encoding.Default.GetString(data, offset, 4));
            offset += 4;
            systemOptions.Option2 = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16);

            return systemOptions;
        }

        public static byte[] RetreiveDateTime()
        {
            List<byte> data = new List<byte>();
            data.Add(Constants.DATETIME);
            return DesCommand.GetCommand(Constants.RETREIVE, data.ToArray());
        }

        public static DateTime RetreiveDateTime(byte[] data)
        {
            int offset = 16;
            int year = Convert.ToInt32(Encoding.Default.GetString(data, offset, 4));
            offset += 4;
            int month = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            int day = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            int hour = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            int minute = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));
            offset += 2;
            int second = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }

        public static byte[] RetreiveRemoteSiteEvents()
        {
            return DesCommand.GetCommand(Constants.EVENT);
        }

        public static RemoteSiteEventLog RetreiveRemoteSiteEvents(byte[] data)
        {
            RemoteSiteEventLog remoteSiteEventLog = new RemoteSiteEventLog();

            int offset = 15;
            if (data[offset] == (byte)'Z') return null;

            remoteSiteEventLog.EventNumber = Convert.ToInt32(Encoding.Default.GetString(data, offset, 2), 16);
            offset += 2;
            char type = (char)data[offset];
            offset++;
            string eventDateTime = Encoding.Default.GetString(data, offset, 8);
            offset += 8;
            eventDateTime += Encoding.Default.GetString(data, offset, 6);
            offset += 6;
            string acmDateTime = Encoding.Default.GetString(data, offset, 8);
            offset += 8;
            acmDateTime += Encoding.Default.GetString(data, offset, 6);
            offset += 6;
            int len = Math.Min(6, data.Length - offset - 3);
            string info = Encoding.Default.GetString(data, offset, len);

            remoteSiteEventLog.Description = "";
            if (type == 'p')
            {
                switch (info[0])
                {
                    case 'C': remoteSiteEventLog.Description = "Channel " + (Convert.ToInt32(info.Substring(1, 2), 16) + 1).ToString("D3"); break;
                    case 'G': remoteSiteEventLog.Description = "Channel " + (Convert.ToInt32(info.Substring(1, 2), 16) + 1).ToString("D3") + " Tag " + (info[3] & 0x0f).ToString("D1"); break;
                    case 'T': remoteSiteEventLog.Description = "Trade code " + (Convert.ToInt32(info.Substring(1, 2), 16) + 1).ToString("D3"); break;
                    case 'S': remoteSiteEventLog.Description = "Schedule " + (Convert.ToInt32(info.Substring(1, 2), 16) + 1).ToString("D3"); break;
                    case 'K': remoteSiteEventLog.Description = "Clock changed"; break;
                    case 'D': remoteSiteEventLog.Description = "Door " + (Convert.ToInt32(info.Substring(1, 2), 16) + 1).ToString("D3"); break;
                    case 'A': remoteSiteEventLog.Description = "Staff access " + (Convert.ToInt32(info.Substring(1, 2), 16) + 1).ToString("D3"); break;
                    case 'O': remoteSiteEventLog.Description = "Options"; break;
                    case 'E': remoteSiteEventLog.Description = "Event enable/disable changed"; break;
                }
            }

            DateTime dateEvent = DateTime.Now;
            DateTime dateACM = DateTime.Now;
            if (!DateTime.TryParseExact(eventDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEvent))
            {
                dateEvent = DateTime.Now;
            }
            if (!DateTime.TryParseExact(acmDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateACM))
            {
                dateACM = DateTime.Now;
            }

            remoteSiteEventLog.DateEvent = dateEvent;
            remoteSiteEventLog.DateACM = dateACM;

            return remoteSiteEventLog;
        }

        public static byte[] ClearEvent(int eventNumber)
        {
            List<byte> data = new List<byte>();
            data.AddRange(Encoding.Default.GetBytes(eventNumber.ToString("X2")));
            return DesCommand.GetCommand(Constants.EVENT_ENABLE);
        }
    }
}
