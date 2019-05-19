using System.Collections.ObjectModel;
using System.Linq;

namespace LMPT.Core.Server.ViewModels
{
    public class Analytics : BaseViewModel
    {
        private const int MaxLogsToShow = 300;

        public Analytics()
        {
            Logs = new ObservableCollection<LogViewModel>();
            Logs.CollectionChanged += (s, e) => NofifyChanged("Logs");
        }

        public ObservableCollection<LogViewModel> Logs { get; set; }
        public int HttpCallPerMinute { get; set; }

        /// Value between 0 and 1;


        public string FooterInfo { get; set; }


        public void AddLog(LogViewModel log)
        {
            if (Logs.Count > MaxLogsToShow) Logs.Remove(Logs.First());
            Logs.Add(log);
        }
    }
}