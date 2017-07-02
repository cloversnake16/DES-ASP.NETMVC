using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using DESCore.General;
using DESCore.Helpers;
using DESCore.Database;
using DESCore.Models;
using DESCore.Sessions;

namespace DESCore.DesCommands
{
    public class CommandHelper
    {
        public static TcpSession GetSession(string deviceId)
        {
            TcpSession session = new TcpSession();
            if (!session.Connect()) session.DataModel = new DataModel { DataType = DataType.Disconnect };
            else session.Send(ConvertHelper.GetBytes(DataType.Login, deviceId));
            return session;
        }

        public static void LogEvent(EventLogModel eventLogModel, EventStatus status, string description)
        {
            try
            {
                eventLogModel.Status = status;
                eventLogModel.EventLog.Description = description;
                EventLogDb.LogEvent(eventLogModel);
            }
            catch { }
        }

        public static ResultModel LoadParameter(SiteParaModel siteParaModel, Json siteModel)
        {
            string deviceId = siteModel.Site.DeviceId;
            EventLogModel eventLogModel = new EventLogModel
            {
                DeviceId = deviceId,
                Event = EventType.RetrieveFromSite,
                EventLog = new EventLog(),
            };

            try
            {
                using (var session = GetSession(deviceId))
                {
                    if (session.DataModel.DataType != DataType.Login)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }

                    ResultModel resultModel = new ResultModel { Status = true };
                    if (siteParaModel.SiteParaType == SiteParaType.Channel)
                    {
                        resultModel = RetrieveChannel(session, siteParaModel.Channel.ChannelIndex, (siteModel.Version >= 4), eventLogModel);
                        if (resultModel.Status) siteParaModel.Channel = (Channel)resultModel.Result;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.Schedule)
                    {
                        resultModel = RetrieveSchedule(session, siteParaModel.Schedule.ScheduleIndex, eventLogModel);
                        if (resultModel.Status) siteParaModel.Schedule = (Schedule)resultModel.Result;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.TradeCode)
                    {
                        resultModel = RetrieveTradeCode(session, siteParaModel.TradeCode.TradeCodeIndex, eventLogModel);
                        if (resultModel.Status) siteParaModel.TradeCode = (TradeCode)resultModel.Result;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.Door)
                    {
                        resultModel = RetrieveDoor(session, siteParaModel.Door.DoorIndex, eventLogModel);
                        if (resultModel.Status) siteParaModel.Door = (Door)resultModel.Result;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.StaffAccess)
                    {
                        resultModel = RetrieveStaffAccess(session, siteParaModel.StaffAccess.StaffAccessIndex, eventLogModel);
                        if (resultModel.Status) siteParaModel.StaffAccess = (StaffAccess)resultModel.Result;
                    }
                    else if (siteParaModel.SiteParaType == SiteParaType.SystemOption)
                    {
                        resultModel = RetrieveSystemOptions(session, eventLogModel);
                        if (resultModel.Status) siteParaModel.SystemOption = (SystemOption)resultModel.Result;
                    }

                    if (!resultModel.Status) return resultModel;

                    return new ResultModel { Status = true, Result = siteParaModel };
                }
            }
            catch (Exception ex)
            {
                LogEvent(eventLogModel, EventStatus.Failure, ex.Message);
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static ResultModel SaveParameter(SiteParaModel siteParaModel, Json siteModel)
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
                using (var session = GetSession(deviceId))
                {
                    if (session.DataModel.DataType != DataType.Login)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }

                    ResultModel resultModel = new ResultModel{Status = true};
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
            }
            catch (Exception ex)
            {
                LogEvent(eventLogModel, EventStatus.Failure, ex.Message);
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static ResultModel RetrieveChannel(TcpSession session, int index, bool isDoorX, EventLogModel eventLogModel)
        {
            #region Channel
            eventLogModel.EventLog.Request = RetreiveCommand.RetreiveChannel(index);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            Channel channel = RetreiveCommand.RetreiveChannel(eventLogModel.EventLog.Response);
            LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetChannelDescription(channel));
            #endregion

            #region Channel Door Extension
            if (isDoorX)
            {
                for (int i = 1; i < 8; i++)
                {
                    eventLogModel.EventLog.Request = RetreiveCommand.RetreiveChannelDoor(index, i);
                    session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    int dr = RetreiveCommand.RetreiveChannelDoor(eventLogModel.EventLog.Response);

                    if (i == 1) channel.Door1 |= (long)dr << 16;
                    else if (i == 2) channel.Door1 |= (long)dr << 32;
                    else if (i == 3) channel.Door1 |= (long)dr << 48;
                    else if (i == 4) channel.Door2 = (long)dr;
                    else if (i == 5) channel.Door2 |= (long)dr << 16;
                    else if (i == 6) channel.Door2 |= (long)dr << 32;
                    else if (i == 7) channel.Door2 |= (long)dr << 48;

                    LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetDoorDescription(channel, i));
                }
            }
            #endregion

            #region Channel Tag
            if (channel.Flat != 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    eventLogModel.EventLog.Request = RetreiveCommand.RetreiveTag(index, i);
                    session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    int tag = RetreiveCommand.RetreiveTag(eventLogModel.EventLog.Response);

                    if (i == 0) channel.Tag1 = tag;
                    else if (i == 1) channel.Tag2 = tag;
                    else if (i == 2) channel.Tag3 = tag;
                    else if (i == 3) channel.Tag4 = tag;
                    else if (i == 4) channel.Tag5 = tag;
                    else if (i == 5) channel.Tag6 = tag;
                    else if (i == 6) channel.Tag7 = tag;
                    else if (i == 7) channel.Tag8 = tag;

                    LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetTagDescription(channel, i));
                }
            }
            #endregion
            return new ResultModel { Status = true, Result = channel };
        }

        public static ResultModel ProgramChannel(TcpSession session, Channel channel, bool isDoorX, EventLogModel eventLogModel)
        {

            #region Channel
            eventLogModel.EventLog.Request = ProgramCommand.ProgramChannel(channel);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetChannelDescription(channel));
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
                    session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetDoorDescription(channel, i));
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
                    session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    LogEvent(eventLogModel, EventStatus.Successful, ChannelDb.GetTagDescription(channel, i));
                }
            }
            #endregion

            return new ResultModel { Status = true, Result = channel };
        }

        public static ResultModel RetrieveSchedule(TcpSession session, int index, EventLogModel eventLogModel)
        {

            #region Schedule
            eventLogModel.EventLog.Request = RetreiveCommand.RetreiveSchedule(index);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            Schedule schedule = RetreiveCommand.RetreiveSchedule(eventLogModel.EventLog.Response);
            LogEvent(eventLogModel, EventStatus.Successful, ScheduleDb.GetDescription(schedule));
            #endregion

            return new ResultModel { Status = true, Result = schedule };
        }

        public static ResultModel ProgramSchedule(TcpSession session, Schedule schedule, EventLogModel eventLogModel)
        {
            #region Schedule
            eventLogModel.EventLog.Request = ProgramCommand.ProgramSchedule(schedule);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            LogEvent(eventLogModel, EventStatus.Successful, ScheduleDb.GetDescription(schedule));
            #endregion

            return new ResultModel { Status = true, Result = schedule };
        }

        public static ResultModel RetrieveTradeCode(TcpSession session, int index, EventLogModel eventLogModel)
        {
            #region TradeCode
            eventLogModel.EventLog.Request = RetreiveCommand.RetreiveTradeCode(index);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            TradeCode tradeCode = RetreiveCommand.RetreiveTradeCode(eventLogModel.EventLog.Response);
            LogEvent(eventLogModel, EventStatus.Successful, TradeCodeDb.GetDescription(tradeCode));
            #endregion

            return new ResultModel { Status = true, Result = tradeCode };
        }

        public static ResultModel ProgramTradeCode(TcpSession session, TradeCode tradeCode, EventLogModel eventLogModel)
        {
            #region TradeCode
            eventLogModel.EventLog.Request = ProgramCommand.ProgramTradeCode(tradeCode);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            LogEvent(eventLogModel, EventStatus.Successful, TradeCodeDb.GetDescription(tradeCode));
            #endregion

            return new ResultModel { Status = true, Result = tradeCode };
        }

        public static ResultModel RetrieveDoor(TcpSession session, int index, EventLogModel eventLogModel)
        {
            #region Door
            eventLogModel.EventLog.Request = RetreiveCommand.RetreiveDoor(index);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            Door door = RetreiveCommand.RetreiveDoor(eventLogModel.EventLog.Response);
            LogEvent(eventLogModel, EventStatus.Successful, DoorDb.GetDescription(door));
            #endregion

            return new ResultModel { Status = true, Result = door };
        }

        public static ResultModel ProgramDoor(TcpSession session, Door door, EventLogModel eventLogModel)
        {
            #region Door
            eventLogModel.EventLog.Request = ProgramCommand.ProgramDoor(door);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            LogEvent(eventLogModel, EventStatus.Successful, DoorDb.GetDescription(door));
            #endregion

            return new ResultModel { Status = true, Result = door };
        }

        public static ResultModel RetrieveStaffAccess(TcpSession session, int index, EventLogModel eventLogModel)
        {
            #region StaffAccess
            eventLogModel.EventLog.Request = RetreiveCommand.RetreiveStaffAccess(index);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            StaffAccess staffAccess = RetreiveCommand.RetreiveStaffAccess(eventLogModel.EventLog.Response);
            LogEvent(eventLogModel, EventStatus.Successful, StaffAccessDb.GetDescription(staffAccess));
            #endregion

            return new ResultModel { Status = true, Result = staffAccess };
        }

        public static ResultModel ProgramStaffAccess(TcpSession session, StaffAccess staffAccess, EventLogModel eventLogModel)
        {
            #region StaffAccess
            eventLogModel.EventLog.Request = ProgramCommand.ProgramStaffAccess(staffAccess);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            LogEvent(eventLogModel, EventStatus.Successful, StaffAccessDb.GetDescription(staffAccess));
            #endregion

            return new ResultModel { Status = true, Result = staffAccess };
        }

        public static ResultModel RetrieveSystemOptions(TcpSession session, EventLogModel eventLogModel)
        {
            #region SystemOption
            eventLogModel.EventLog.Request = RetreiveCommand.RetreiveSystemOptions();
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            SystemOption systemOption = RetreiveCommand.RetreiveSystemOptions(eventLogModel.EventLog.Response);
            LogEvent(eventLogModel, EventStatus.Successful, SystemOptionDb.GetDescription(systemOption));
            #endregion

            return new ResultModel { Status = true, Result = systemOption };
        }

        public static ResultModel ProgramSystemOptions(TcpSession session, SystemOption systemOption, EventLogModel eventLogModel)
        {
            #region SystemOption
            eventLogModel.EventLog.Request = ProgramCommand.ProgramSystemOptions(systemOption);
            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
            if (session.DataModel.DataType == DataType.NoReply)
            {
                LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
            }
            eventLogModel.EventLog.Response = session.RecvBytes;

            LogEvent(eventLogModel, EventStatus.Successful, SystemOptionDb.GetDescription(systemOption));
            #endregion

            return new ResultModel { Status = true, Result = systemOption };
        }

        public static ResultModel RetrieveDateTime(string deviceId)
        {
            EventLogModel eventLogModel = new EventLogModel
            {
                DeviceId = deviceId,
                Event = EventType.RetrieveFromSite,
                EventLog = new EventLog(),
            };

            try
            {
                using (var session = GetSession(deviceId))
                {
                    if (session.DataModel.DataType != DataType.Login)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }

                    #region DateTime
                    eventLogModel.EventLog.Request = RetreiveCommand.RetreiveDateTime();
                    session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    DateTime dateTime = RetreiveCommand.RetreiveDateTime(eventLogModel.EventLog.Response);
                    LogEvent(eventLogModel, EventStatus.Successful, dateTime.ToString("yyyy-MM-dd HH:mm:ss ddd"));
                    #endregion

                    return new ResultModel { Status = true, Result = dateTime.ToString("yyyy-MM-dd HH:mm:ss ddd") };
                }
            }
            catch (Exception ex)
            {
                LogEvent(eventLogModel, EventStatus.Failure, ex.Message);
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static ResultModel ProgramDateTime(string deviceId, DateTime datetime)
        {
            EventLogModel eventLogModel = new EventLogModel
            {
                DeviceId = deviceId,
                Event = EventType.ProgramToSite,
                EventLog = new EventLog(),
            };

            try
            {
                using (var session = GetSession(deviceId))
                {
                    if (session.DataModel.DataType != DataType.Login)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }

                    #region DateTime
                    eventLogModel.EventLog.Request = ProgramCommand.ProgramDateTime(datetime);
                    session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    LogEvent(eventLogModel, EventStatus.Successful, datetime.ToString("yyyy-MM-dd HH:mm:ss ddd"));
                    #endregion

                    return new ResultModel { Status = true, Result = datetime.ToString("yyyy-MM-dd HH:mm:ss ddd") };
                }
            }
            catch (Exception ex)
            {
                LogEvent(eventLogModel, EventStatus.Failure, ex.Message);
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static void RetrieveEvents(string deviceId)
        {
            List<RemoteSiteEventModel> listRemoteSiteEventModel = new List<RemoteSiteEventModel>();

            try
            {
                using (var session = GetSession(deviceId))
                {
                    if (session.DataModel.DataType != DataType.Login)
                    {
                        RemoteSiteEventModel remoteEventModel = new RemoteSiteEventModel
                        {
                            DeviceId = deviceId,
                            Status = EventStatus.Failure,
                            RemoteSiteEventLog = new RemoteSiteEventLog { Description = session.DataModel.DataType.ToString() },
                        };
                        listRemoteSiteEventModel.Add(remoteEventModel);
                        EventLogDb.SaveRemoteEvent(listRemoteSiteEventModel);
                        return;
                    }

                    #region RemoteSiteEvent
                    char type = '0';
                    do
                    {
                        RemoteSiteEventModel remoteEventModel = new RemoteSiteEventModel
                        {
                            DeviceId = deviceId,
                            Status = EventStatus.Successful,
                            RemoteSiteEventLog = new RemoteSiteEventLog(),
                        };
                        
                        type = '0';
                        byte[] request = RetreiveCommand.RetreiveRemoteSiteEvents();
                        session.Send(ConvertHelper.GetBytes(DataType.DataReceived, remoteEventModel.RemoteSiteEventLog.Request));
                        if (session.DataModel.DataType == DataType.NoReply) break;

                        byte[] response = session.RecvBytes;
                        remoteEventModel.RemoteSiteEventLog = RetreiveCommand.RetreiveRemoteSiteEvents(response);
                        if (remoteEventModel.RemoteSiteEventLog == null) break;

                        remoteEventModel.RemoteSiteEventLog.Request = request;
                        remoteEventModel.RemoteSiteEventLog.Response = response;
                        listRemoteSiteEventModel.Add(remoteEventModel);

                        if (type != '0')
                        {
                            request = RetreiveCommand.ClearEvent(remoteEventModel.RemoteSiteEventLog.EventNumber);
                            session.Send(ConvertHelper.GetBytes(DataType.DataReceived, request));
                        }

                    } while (type != '0');
                    #endregion

                    return;
                }
            }
            catch (Exception) { }
            if (listRemoteSiteEventModel.Count > 0) EventLogDb.SaveRemoteEvent(listRemoteSiteEventModel);
        }

        public static ResultModel ProgramEventsEnable(string deviceId, EventEnableModel eventEnableModel)
        {
            EventLogModel eventLogModel = new EventLogModel
            {
                DeviceId = deviceId,
                Event = EventType.ProgramToSite,
                EventLog = new EventLog(),
            };

            try
            {
                using (var session = GetSession(deviceId))
                {
                    if (session.DataModel.DataType != DataType.Login)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }

                    #region EventEnableModel
                    eventLogModel.EventLog.Request = ProgramCommand.ProgramEventEnable(eventEnableModel);
                    session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    LogEvent(eventLogModel, EventStatus.Successful, EventEnableDb.GetDescription(eventEnableModel));
                    #endregion

                    return new ResultModel { Status = true, Result = eventEnableModel };
                }
            }
            catch (Exception ex)
            {
                LogEvent(eventLogModel, EventStatus.Failure, ex.Message);
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public static ResultModel OpenDoor(string deviceId, int index)
        {
            EventLogModel eventLogModel = new EventLogModel
            {
                DeviceId = deviceId,
                Event = EventType.ProgramToSite,
                EventLog = new EventLog(),
            };

            try
            {
                using (var session = GetSession(deviceId))
                {
                    if (session.DataModel.DataType != DataType.Login)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }

                    #region EventEnableModel
                    eventLogModel.EventLog.Request = ProgramCommand.OpenDoor(index);
                    session.Send(ConvertHelper.GetBytes(DataType.DataReceived, eventLogModel.EventLog.Request));
                    if (session.DataModel.DataType == DataType.NoReply)
                    {
                        LogEvent(eventLogModel, EventStatus.Failure, session.DataModel.DataType.ToString());
                        return new ResultModel { Status = false, Result = session.DataModel.DataType.ToString() };
                    }
                    eventLogModel.EventLog.Response = session.RecvBytes;

                    LogEvent(eventLogModel, EventStatus.Successful, "Door's Index:" + index);
                    #endregion

                    return new ResultModel { Status = false, Result = "Success to open Door" + Convert.ToString(index+1) };
                }
            }
            catch (Exception ex)
            {
                LogEvent(eventLogModel, EventStatus.Failure, ex.Message);
                return new ResultModel { Status = true, Result = ex.Message };
            }
        }
    }
}
