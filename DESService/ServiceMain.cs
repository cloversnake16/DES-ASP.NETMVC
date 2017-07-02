using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DESCore.Sessions;

namespace DESService
{
    public partial class ServiceMain : ServiceBase
    {
        private DesServer _desServer = new DesServer();
        private WebServer _webServer = new WebServer();

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _desServer.Listen(int.Parse(ConfigurationManager.AppSettings["ListenPort"]));
            _webServer.Listen(5000);
        }

        protected override void OnStop()
        {
            _desServer.Stop();
            _webServer.Stop();
        }
    }
}
