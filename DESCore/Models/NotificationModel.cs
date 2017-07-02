using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public enum NotificationType { Notifications, BackTasks, RemoveNotification, RetryBackTask, RemoveBackTask, }

    public class NotificationModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public NotificationType NotificationType { get; set; }
        public int Id { get; set; }
    }
}
