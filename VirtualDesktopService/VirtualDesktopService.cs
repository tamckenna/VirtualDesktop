using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace VirtualDesktopService
{
    partial class VirtualDesktopService : ServiceBase
    {

        private ProcessDetail PD;
        public VirtualDesktopService()
        {
            log(DateTime.Now.ToLocalTime().ToLongTimeString() + " VirtualDesktopService initialized!");
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.killDuplicateExecutions();
            string appPath = this.getCurrentDirectoryPath() + @"\" + "VirtualDesktop.exe";

            PD = new ProcessDetail();
            PD.appPath = appPath;
            PD = ProcessExtensions.StartProcessAsCurrentUser(PD);
        }

        protected override void OnStop()
        {
            // Kill Process by ID (PD.processId)
            this.KillProcessById(PD.processId);
        }

        protected string getCurrentDirectoryPath()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            return strWorkPath;
        }

        protected void KillProcessById(int pid)
        {
            Process[] process = Process.GetProcesses();

            foreach (Process prs in process)
            {
                if (prs.Id == pid)
                {
                    prs.Kill();
                    break;
                }
            }
        }

        protected void killDuplicateExecutions()
        {
            foreach (var process in Process.GetProcessesByName("VirtualDesktop")){ process.Kill(); }
        }

        protected void log(string text)
        {
            Console.WriteLine(text);
            File.WriteAllText(getCurrentDirectoryPath() + @"\" + "VirtualDesktop.log", text);
        }
    }
}
