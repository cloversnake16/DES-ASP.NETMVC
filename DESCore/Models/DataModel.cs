using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public enum DataType 
    {
        NoReply, Disconnect, Login, NoExist, Busy, DataReceived, 
        Accept, Send, Receive,
        Log,
        Info, Error,
        Successful, Failure,
        Device, Listen,
        Version, Channel,
        Notifications, BackTasks, RemoveNotification, RemoveBackTask,
    };
    public class DataModel
    {
        public DataType DataType { get; set; }
        public string Data { get; set; }
    }
}
