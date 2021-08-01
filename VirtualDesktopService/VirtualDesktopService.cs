using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDesktopService
{
    partial class VirtualDesktopService : ServiceBase
    {

        private ProcessDetail PD;
        public VirtualDesktopService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            PD = new ProcessDetail();
            PD.appPath = "VirtualDesktop.exe";
            PD = ProcessExtensions.StartProcessAsCurrentUser(PD);
        }

        protected override void OnStop()
        {
            // TODO: Kill Process by ID (PD.processId)

        }
    }
}
