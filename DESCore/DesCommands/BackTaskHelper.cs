using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.General;
using DESCore.Helpers;
using DESCore.Database;
using DESCore.Models;
using DESCore.Sessions;

namespace DESCore.DesCommands
{
    public class BackTaskHelper
    {
        public static ResultModel SaveParameter(DesSession session, SiteParaModel siteParaModel, Json siteModel)
        {
            string deviceId = siteModel.Site.DeviceId;
            EventLogModel eventLogModel = new EventLogModel
            {
                DeviceId = deviceId,
                Event = EventType.ProgramToSite,
                EventLog = new EventLog(),
            };

            try
            {
                ResultModel resultModel = new ResultModel { Status = true };
                if (siteParaModel.SiteParaType == SiteParaType.Channel)
                {
                    resultModel = ProgramChannel(session, siteParaModel.Channel, (siteModel.Version >= 4), eventLogModel);
                }
                else if (siteParaModel.SiteParaType == SiteParaType.Schedule)
                {
                    resultModel = ProgramSchedule(session, siteParaModel.Schedule, eventLogModel);
                }
                else if (siteParaModel.SiteParaType == SiteParaType.TradeCode)
                {
                    resultModel = ProgramTradeCode(session, siteParaModel.TradeCode, eventLogModel);
                }
                else if (siteParaModel.SiteParaType == SiteParaType.Door)
                {
                    resultModel = ProgramDoor(session, siteParaModel.Door, eventLogModel);
                }
                else if (siteParaModel.SiteParaType == SiteParaType.StaffAccess)
                {
                    resultModel = ProgramStaffAccess(session, siteParaModel.StaffAccess, eventLogModel);
                }
                else if (siteParaModel.SiteParaType == SiteParaType.SystemOption)
                {
                    resultModel = ProgramSystemOptions(session, siteParaModel.SystemOption, eventLogModel);
                }

                if (!resultModel.Status) return resultModel;

                return new ResultModel { Status = true, Result = siteParaModel };
            }
            catch (Exception ex)
            {
                CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, ex.Message);
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static ResultModel ProgramChannel(DesSession session, Channel channel, bool isDoorX, EventLogModel eventLogModel)
        {

            #region Channel
            eventLogModel.EventLog.Request = ProgramCommand.ProgramChannel(channel);
            session.Send(ConvertHelper.Encoder(eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            CommandHelper.LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetChannelDescription(channel));
            #endregion

            #region Channel Door Extension
            if (isDoorX)
            {
                for (int i = 1; i < 8; i++)
                {
                    int door = 0;
                    if (i == 1) door = (int)((channel.Door1 >> 16) & 0xFFFF);
                    else if (i == 2) door = (int)((channel.Door1 >> 32) & 0xFFFF);
                    else if (i == 3) door = (int)((channel.Door1 >> 48) & 0xFFFF);
                    else if (i == 4) door = (int)(channel.Door2 & 0xFFFF);
                    else if (i == 5) door = (int)((channel.Door2 >> 16) & 0xFFFF);
                    else if (i == 6) door = (int)((channel.Door2 >> 32) & 0xFFFF);
                    else if (i == 7) door = (int)((channel.Door2 >> 48) & 0xFFFF);

                    eventLogModel.EventLog.Request = ProgramCommand.ProgramChannelDoor(channel.ChannelIndex, i, door);
                    session.Send(ConvertHelper.Encoder(eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    CommandHelper.LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetDoorDescription(channel, i));
                }
            }
            #endregion

            #region Channel Tag
            if (channel.Flat != 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    int tag = 0;
                    if (i == 0) tag = channel.Tag1;
                    else if (i == 1) tag = channel.Tag2;
                    else if (i == 2) tag = channel.Tag3;
                    else if (i == 3) tag = channel.Tag4;
                    else if (i == 4) tag = channel.Tag5;
                    else if (i == 5) tag = channel.Tag6;
                    else if (i == 6) tag = channel.Tag7;
                    else if (i == 7) tag = channel.Tag8;
                    eventLogModel.EventLog.Request = ProgramCommand.ProgramTag(channel.ChannelIndex, i, Convert.ToInt32(tag));
                    session.Send(ConvertHelper.Encoder(eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    CommandHelper.LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetTagDescription(channel, i));
                }
            }
            #endregion

            return new ResultModel { Status = true, Result = channel };
        }

        public static ResultModel ProgramSchedule(DesSession session, Schedule schedule, EventLogModel eventLogModel)
        {
            #region Schedule
            eventLogModel.EventLog.Request = ProgramCommand.ProgramSchedule(schedule);
            session.Send(ConvertHelper.Encoder(eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            CommandHelper.LogEvent(eventLogModel, EventStatus.Successful, ScheduleDb.GetDescription(schedule));
            #endregion

            return new ResultModel { Status = true, Result = schedule };
        }

        public static ResultModel ProgramTradeCode(DesSession session, TradeCode tradeCode, EventLogModel eventLogModel)
        {
            #region TradeCode
            eventLogModel.EventLog.Request = ProgramCommand.ProgramTradeCode(tradeCode);
            session.Send(ConvertHelper.Encoder(eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            CommandHelper.LogEvent(eventLogModel, EventStatus.Successful, TradeCodeDb.GetDescription(tradeCode));
            #endregion

            return new ResultModel { Status = true, Result = tradeCode };
        }

        public static ResultModel ProgramDoor(DesSession session, Door door, EventLogModel eventLogModel)
        {
            #region Door
            eventLogModel.EventLog.Request = ProgramCommand.ProgramDoor(door);
            session.Send(ConvertHelper.Encoder(eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            CommandHelper.LogEvent(eventLogModel, EventStatus.Successful, DoorDb.GetDescription(door));
            #endregion

            return new ResultModel { Status = true, Result = door };
        }

        public static ResultModel ProgramStaffAccess(DesSession session, StaffAccess staffAccess, EventLogModel eventLogModel)
        {
            #region StaffAccess
            eventLogModel.EventLog.Request = ProgramCommand.ProgramStaffAccess(staffAccess);
            session.Send(ConvertHelper.Encoder(eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            CommandHelper.LogEvent(eventLogModel, EventStatus.Successful, StaffAccessDb.GetDescription(staffAccess));
            #endregion

            return new ResultModel { Status = true, Result = staffAccess };
        }

        public static ResultModel ProgramSystemOptions(DesSession session, SystemOption systemOption, EventLogModel eventLogModel)
        {
            #region SystemOption
            eventLogModel.EventLog.Request = ProgramCommand.ProgramSystemOptions(systemOption);
            session.Send(ConvertHelper.Encoder(eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                CommandHelper.LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            CommandHelper.LogEvent(eventLogModel, EventStatus.Successful, SystemOptionDb.GetDescription(systemOption));
            #endregion

            return new ResultModel { Status = true, Result = systemOption };
        }
    }
}
