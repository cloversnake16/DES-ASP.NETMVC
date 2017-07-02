using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class ChannelDb
    {
        public static ResultModel Save(DESEntities db, Channel channel)
        {
            try
            {
                Channel ch = db.Channels.Where(r => r.SiteDataId == channel.SiteDataId &&
                    r.ChannelIndex == channel.ChannelIndex).FirstOrDefault();
                if (ch == null)
                {
                    channel.DateUpdated = DateTime.Now;
                    ch = db.Channels.Add(channel);
                }
                else
                {
                    ch.Flat = channel.Flat;
                    ch.PPP = channel.PPP;
                    ch.Door1 = channel.Door1;
                    ch.Door2 = channel.Door2;
                    ch.Tag1 = channel.Tag1;
                    ch.Tag2 = channel.Tag2;
                    ch.Tag3 = channel.Tag3;
                    ch.Tag4 = channel.Tag4;
                    ch.Tag5 = channel.Tag5;
                    ch.Tag6 = channel.Tag6;
                    ch.Tag7 = channel.Tag7;
                    ch.Tag8 = channel.Tag8;
                    ch.DateUpdated = DateTime.Now;
                    db.Entry(ch).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return new ResultModel { Status = true, };
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; }
        }

        public static string GetDescription(Channel channel)
        {
            return "ChannelIndex:" + channel.ChannelIndex + ", " +
                "Flat:" + channel.Flat + ", " +
                "PPP:" + channel.PPP + ", " +
                "Door:" + channel.Door1 + ", " +
                channel.Door2 + ", " +
                "Tag:" + channel.Tag1 + "," +
                channel.Tag2 + "," +
                channel.Tag3 + "," +
                channel.Tag4 + "," +
                channel.Tag5 + "," +
                channel.Tag6 + "," +
                channel.Tag7 + "," +
                channel.Tag8;
        }

        public static string GetChannelDescription(Channel channel)
        {
            return "ChannelIndex:" + channel.ChannelIndex + ", " +
                "Flat:" + channel.Flat + ", " +
                "PPP:" + channel.PPP + ", " +
                "Door:" + channel.Door1;
        }

        public static string GetDoorDescription(Channel channel, int doorIndex)
        {
            int door = 0;
            if (doorIndex == 0) door = (int)(channel.Door1 & 0xFFFF);
            else if (doorIndex == 1) door = (int)((channel.Door1 >> 16) & 0xFFFF);
            else if (doorIndex == 2) door = (int)((channel.Door1 >> 32) & 0xFFFF);
            else if (doorIndex == 3) door = (int)((channel.Door1 >> 48) & 0xFFFF);
            else if (doorIndex == 4) door = (int)(channel.Door2 & 0xFFFF);
            else if (doorIndex == 5) door = (int)((channel.Door2 >> 16) & 0xFFFF);
            else if (doorIndex == 6) door = (int)((channel.Door2 >> 32) & 0xFFFF);
            else if (doorIndex == 7) door = (int)((channel.Door2 >> 48) & 0xFFFF);
            return "ChannelIndex:" + channel.ChannelIndex + ", door" + (doorIndex + 1) + ": " + door;
        }

        public static string GetTagDescription(Channel channel, int tagIndex)
        {
            int tag = 0;
            if (tagIndex == 0) tag = channel.Tag1;
            else if (tagIndex == 1) tag = channel.Tag2;
            else if (tagIndex == 2) tag = channel.Tag3;
            else if (tagIndex == 3) tag = channel.Tag4;
            else if (tagIndex == 4) tag = channel.Tag5;
            else if (tagIndex == 5) tag = channel.Tag6;
            else if (tagIndex == 6) tag = channel.Tag7;
            else if (tagIndex == 7) tag = channel.Tag8;
            return "ChannelIndex:" + channel.ChannelIndex + ", Tag" + (tagIndex + 1) + ": " + tag;
        }
    }
}
