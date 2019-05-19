using System;

namespace LMPT.Core.Server.ViewModels
{
    public class ViewModelMediator
    {
        public event Action<ViewModelNotification, object> OnNotfication;

        public void Send(ViewModelNotification type, object args)
        {
            OnNotfication(type, args);
        }
    }
}