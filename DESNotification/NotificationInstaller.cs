using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace DESNotification
{
    [RunInstaller(true)]
    public partial class NotificationInstaller : System.Configuration.Install.Installer
    {
        public NotificationInstaller()
        {
            InitializeComponent();
        }

        private void NotificationInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            string folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            System.Diagnostics.Process.Start(folder + "\\DESNotification.exe");
        }
    }
}
