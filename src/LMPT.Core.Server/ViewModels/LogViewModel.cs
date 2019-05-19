namespace LMPT.Core.Server.ViewModels
{
    public class LogViewModel
    {
        public string DateTimeFormatted { get; set; } = string.Empty;
        public string Source { get; set; }  = string.Empty;
        public string Log { get; set; }  = string.Empty;
        public int LogLevel { get; set; }
    }
}