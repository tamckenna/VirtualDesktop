using System.ServiceProcess;

namespace VirtualDesktopService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new VirtualDesktopService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
