using LMPT.Core.Contract.FromLiveMe;

namespace LMPT.Core.Server.ViewModels
{
    public class ListViewUserCard : BaseViewModel
    {
        public UserInfo UserInfo { get; set; }
        public LiveMeUser LiveMeUser { get; set; }
        public string LastSeen { get; set; }
        public bool IsBookmarked { get; set; }
    }
}