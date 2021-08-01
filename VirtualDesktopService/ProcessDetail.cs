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
