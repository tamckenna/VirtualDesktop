using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDesktopService
{
    public class ProcessDetail
    {
        public string appPath { get; set; }
        public string cmdLine { get; set; }
        public string workDir { get; set; }
        public bool visible { get; set; }
        public int processId { get; set; }
    }
}
