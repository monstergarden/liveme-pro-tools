using System;

namespace LMPT.Core.Server.ViewModels
{
    public class FooterViewModel : BaseViewModel
    {
        public string FooterInfo { get; set; } = string.Empty;
        public double ProgressBarProgress { get; set; }
        public bool DisplayProgressBar { get; set; }


        public void SetProgress(double percentage)
        {
            DisplayProgressBar = true;
            ProgressBarProgress = Math.Min(percentage, 1.0);
        }
    }
}