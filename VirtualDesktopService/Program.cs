using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.ServiceProcess;

namespace VirtualDesktopService
{
    static class Program
    {
        static void Main()
        {
            log(DateTime.Now.ToLocalTime().ToLongTimeString() + " Program.Main() started!");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new VirtualDesktopService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        static void log(string text)
        {
            Console.WriteLine(text);
            File.WriteAllText(getCurrentDirectoryPath() + @"\" + "VirtualDesktop.log", text);
        }

        static string getCurrentDirectoryPath()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            return strWorkPath;
        }
    }
}
