using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DESNotification.Models;

namespace DESNotification.Controls
{
    public partial class NotifyControl : UserControl
    {
        public delegate void DelegateCancel(object sender);
        public DelegateCancel RetryControl { get; set; }
        public DelegateCancel CancelControl { get; set; }
        public Notification Notification { get; set; }
        private NotificationType _notificationType;

        public NotifyControl(Notification notification, NotificationType notificationType)
        {
            Notification = notification;
            _notificationType = notificationType;
            InitializeComponent();
        }

        public void SetNotification(Notification notification)
        {
            Notification = notification;
            labelTitle.Text = Notification.Title;
            labelContents.Text = Notification.Contents + Environment.NewLine + "Time: " + notification.DateUpdated;
            if (_notificationType == NotificationType.BackTasks) buttonRetry.Visible = true;
        }

        private void NotifyControl_Load(object sender, EventArgs e)
        {
            SetNotification(Notification);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (CancelControl != null) CancelControl(this);
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            if (RetryControl != null) RetryControl(this);
        }
    }
}
