using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using DESNotification.Models;
using DESNotification.Helpers;
using DESNotification.Controls;

namespace DESNotification
{
    public partial class FormNotification : Form
    {
        public delegate void DelegateNotify(DataModel dataModel);
        private bool _exit = false;
        private NotificationType _notificationType = NotificationType.Notifications;
        private DateTime _lastNotification = DateTime.Today;
        private Size _size = new Size(488, 546);

        public FormNotification()
        {
            InitializeComponent();
        }

        private void FormNotification_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            Hide();
            Text = _notificationType.ToString();
            new Task(PostNotifcation).Start();
        }
        
        private void FormNotification_FormClosing(object sender, FormClosingEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            Hide();
            if (!_exit) e.Cancel = true;
        }

        private void notificationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_notificationType != NotificationType.Notifications)
            {
                Controls.Clear();
                _notificationType = NotificationType.Notifications;
                Text = _notificationType.ToString();
            }
            Show();
            WindowState = FormWindowState.Normal;
            Size = _size;
            ArrangeControl();
        }

        private void backgroundTasksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_notificationType != NotificationType.BackTasks)
            {
                Controls.Clear();
                _notificationType = NotificationType.BackTasks;
                Text = _notificationType.ToString();
            }
            new Task(PostNotifcation).Start();
            Show();
            WindowState = FormWindowState.Normal;
            Size = _size;
            ArrangeControl();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Process.Start(folder + "\\DESSettings.exe");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _exit = true;
            Application.Exit();
        }

        private async void PostNotifcation()
        {
            string username = RegistryHelper.ReadRegistryKey("Software\\DES", "UserName");
            string password = RegistryHelper.ReadRegistryKey("Software\\DES", "Password");

            string url = "http://89.38.146.22/des/api/notification";
            DataModel dataModel = await HttpHelper.Post(url, new NotificationModel
            {
                UserName = username,
                Password = password,
                NotificationType = _notificationType,
            });
            if (dataModel != null) Notify(dataModel);

            await Task.Delay(1000);
            new Task(PostNotifcation).Start();
        }

        private async void PostNotifcationE(NotificationType notificationType, int id)
        {
            string username = RegistryHelper.ReadRegistryKey("Software\\DES", "UserName");
            string password = RegistryHelper.ReadRegistryKey("Software\\DES", "Password");

            string url = "http://89.38.146.22/des/api/notification";
            DataModel dataModel = await HttpHelper.Post(url, new NotificationModel
            {
                UserName = username,
                Password = password,
                NotificationType = notificationType,
                Id = id,
            });
            if (dataModel != null) Notify(dataModel);
        }

        private void Notify(DataModel dataModel)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new DelegateNotify(Notify), dataModel);
                    return;
                }

                if ((dataModel.DataType == DataType.Notifications && _notificationType == NotificationType.Notifications) ||
                    (dataModel.DataType == DataType.BackTasks && _notificationType == NotificationType.BackTasks))
                {
                    Notification[] notifications = ConvertHelper.GetObject<Notification[]>(dataModel);

                    if (notifications != null)
                    {
                        bool changed = false;

                        int index = 0;
                        while (index < Controls.Count)
                        {
                            NotifyControl notifyControl = (NotifyControl)Controls[index];
                            var notification = notifications.Where(r => r.Id == notifyControl.Notification.Id).FirstOrDefault();
                            if (notification == null)
                            {
                                Controls.Remove(notifyControl);
                                changed = true;
                                continue;
                            }
                            else
                            {
                                notifyControl.SetNotification(notification);
                                index++;
                            }
                        }

                        foreach (var notification in notifications)
                        {
                            if (!ExistsContol(notification))
                            {
                                NotifyControl notifyControl = new NotifyControl(notification, _notificationType);
                                if (_notificationType == NotificationType.BackTasks) notifyControl.RetryControl += RetryControl;
                                notifyControl.CancelControl += CancelControl;
                                Controls.Add(notifyControl);
                                changed = true;
                                if((_lastNotification - notification.DateUpdated).TotalMilliseconds < 0)
                                {
                                    _lastNotification = notification.DateUpdated;
                                    notifyIcon.ShowBalloonTip(1000, notification.Title, notification.Contents, ToolTipIcon.Info);
                                }

                            }
                        }
                        if(changed) ArrangeControl();
                    }
                }
                else if (dataModel.DataType == DataType.Failure)
                {
                    string error = ConvertHelper.GetObject<string>(dataModel);
                }

                Cursor = Cursors.Default;
            }
            catch { }
        }

        private bool ExistsContol(Notification notification)
        {
            foreach (var control in Controls)
            {
                NotifyControl notifyControl = (NotifyControl)control;
                if (notification.Id == notifyControl.Notification.Id) return true;
            }
            return false;
        }

        private void ArrangeControl()
        {
            for (var i = 0; i < Controls.Count; i++ )
            {
                Controls[i].Left = (Width - Controls[i].Width - 20) / 2;
                if (i == 0) Controls[i].Top = 0;
                else Controls[i].Top = Controls[i - 1].Bottom + 10;
            }
        }

        private void RetryControl(object sender)
        {
            Cursor = Cursors.WaitCursor;

            NotifyControl notifyControl = (NotifyControl)sender;
            if (_notificationType == NotificationType.BackTasks)
            {
                PostNotifcationE(NotificationType.RetryBackTask, notifyControl.Notification.Id);
            }
        }

        private void CancelControl(object sender)
        {
            Cursor = Cursors.WaitCursor;

            NotifyControl notifyControl = (NotifyControl)sender;
            if (_notificationType == NotificationType.Notifications)
            {
                PostNotifcationE(NotificationType.RemoveNotification, notifyControl.Notification.Id);
            }
            else if (_notificationType == NotificationType.BackTasks)
            {
                PostNotifcationE(NotificationType.RemoveBackTask, notifyControl.Notification.Id);
            }
        }
    }
}
