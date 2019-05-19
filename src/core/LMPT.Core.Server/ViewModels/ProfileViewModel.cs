using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Contract.FromLiveMe;
using LMPT.Core.Services;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.LivemeApi;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Server.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly DataAccess _dataAccess;
        private readonly LivemeApiProvider _liveMeApi;
        private readonly ILogger<MainViewModel> _logger;


        public ProfileViewModel(
            ILogger<MainViewModel> logger,
            DataAccess dataAccess,
            LivemeApiProvider lmProvider
        )
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _liveMeApi = lmProvider;


            Replays = new ObservableCollection<ReplayViewModel>();
            Replays.CollectionChanged += (s, e) => NofifyChanged("Replays");
        }

        public string Loading { get; private set; }
        public LiveMeUser User { get; private set; }
        public bool IsBookmarked { get; private set; }
        public MainViewStatus ViewStatus { get; set; }
        public ObservableCollection<ReplayViewModel> Replays { get; set; }


        public string GetFullCountryName()
        {
            var code = User.UserInfo.countryCode;
            var fullname = CountryCodes.GetFullName(code);
            return fullname;
        }

        public void ToggleBookmark()
        {
            _dataAccess.ToggleBookmark(User);
            IsBookmarked = !IsBookmarked;
        }

        public async Task LoadUser(string uid, CancellationToken token)
        {
            try
            {
                Replays.Clear();

                ViewStatus = MainViewStatus.LoadingProfile;

                try
                {
                    Loading = uid;
                    User = await _liveMeApi.GetUserInfo(uid, token).ConfigureAwait(true);
                    IsBookmarked = _dataAccess.IsBookmarked(uid);
                }
                catch (Exception)
                {
                    ViewStatus = MainViewStatus.ProfileNotFound;
                    return;
                }

                _dataAccess.SetProfileToSeen(uid);


                await LoadReplays(uid, token);
            }

            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            NofifyChanged();
        }

        private async Task LoadReplays(string uid, CancellationToken token)
        {
            ViewStatus = MainViewStatus.LoadingReplays;


            // first the cache will be read, so we can determine
            // between new replays the user never saw before.
            // Than the Liveme Api will be called, which cases the 
            // cache to update.
            var replayCache = _dataAccess.FindReplaysFromUid(uid).ToList();
            _logger.LogInformation($"Found in Cache: {replayCache.Count}");


            var replays = await _liveMeApi.GetUsersReplays(uid, token, 1, 10).ConfigureAwait(true);
            var onlyInCache = replayCache.Where(x => !replays.Any(y => y.Vid == x.VId)).ToList();

            if (replays.Count == 0 && onlyInCache.Count == 0)
            {
                ViewStatus = MainViewStatus.NoReplaysFound;
                return;
            }

            ViewStatus = MainViewStatus.ReplaysFound;


            foreach (var r in replays) Replays.Add(new ReplayViewModel(r, replayCache));
            foreach (var r in onlyInCache) Replays.Add(new ReplayViewModel(r));

            var index = 1;
            while (replays.Count == 10 && index < 3)
            {
                index++;


                replays = await _liveMeApi
                    .GetUsersReplays(uid, CancellationToken.None, index, 10);

                foreach (var r in replays)
                {
                    if (onlyInCache.Any(x => x.VId == r.Vid)) Replays.Remove(Replays.First(x => x.Vid == r.Vid));
                    Replays.Add(new ReplayViewModel(r, replayCache));
                }
            }
        }


        public void SetReplayToWatched(string vid)
        {
            var rvm = Replays.First(x => x.Vid == vid);
            _dataAccess.SetReplayToWatched(vid);
            rvm.Watched = true;
        }
    }
}