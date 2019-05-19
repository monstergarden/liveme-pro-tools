using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB.Cache;
using LMPT.Core.Contract.FromLiveMe;
using LMPT.Core.Server.Shared;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.Helper;
using LMPT.Core.Services.LivemeApi;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Server.ViewModels
{
    public class FansOrFollowingsViewModel : BaseViewModel, IDisposable
    {

        private const int MaxPageSize = 15;
        public ObservableCollection<ListViewUserCard> Users { get; set; }
        public ListWindowPageType PageType { get; set; }
        public string? UserId { get; set; }
        public LiveMeUser? OfUser { get; set; }
        private readonly CancellationTokenSource _cts;
        private readonly DataAccess _dataAccess;

        private readonly LivemeApiProvider _livemeApiProvider;
        private readonly ILogger<FansOrFollowingsViewModel> _logger;
        private readonly ViewModelMediator _viewModelMediator;

        private int _lastIndex;

        public FansOrFollowingsViewModel(
            ILogger<FansOrFollowingsViewModel> logger,
            LivemeApiProvider livemeApiProvider,
            ViewModelMediator viewModelMediator,
            DataAccess dataAccess)
        {
            _cts = new CancellationTokenSource();
            _logger = logger;
            _logger.LogInformation("Ctor called.");
            _livemeApiProvider = livemeApiProvider;
            _viewModelMediator = viewModelMediator;
            _dataAccess = dataAccess;
            Users = new ObservableCollection<ListViewUserCard>();
            Users.CollectionChanged += (s, e) => NofifyChanged();
        }



        public void Dispose()
        {
            _logger.LogDebug($"Dispose {GetType()}");
            _cts?.Cancel();
            _cts?.Dispose();
        }


        public async Task Load(ListWindowPageType pageType)
        {
            PageType = pageType;
            OfUser = await _livemeApiProvider
                .GetUserInfo(UserId, CancellationToken.None)
                .ConfigureAwait(true);

            NofifyChanged();

            await LoadMore().ConfigureAwait(true);
        }

        public async Task LoadMore()
        {
            var userId = OfUser?.UserInfo?.uid;
            if(userId == null) return;

            if (PageType == ListWindowPageType.Fans)
                await LoadIncremental(_livemeApiProvider.GetFans, userId);
            else
                await LoadIncremental(_livemeApiProvider.GetFollowing, userId);
        }

        private async Task LoadIncremental(Func<string, CancellationToken, int, int, Task<List<UserInfo>>> apiFunc,
            string userId)
        {
            var iterations = _lastIndex;
            var maxIndex = _lastIndex + 2;
            IEnumerable<UserInfo> users;
            do
            {
                iterations++;
                _lastIndex = iterations;
                var index = (iterations - 1) * MaxPageSize + 1;
                users = await apiFunc(userId, _cts.Token, index, MaxPageSize);
                foreach (var userInfo in users)
                {
                    var watched = _dataAccess.FindProfileWatched(userInfo.uid);
                    var lastSeen = LastSeen(watched);

                    var bookmarked = _dataAccess.IsBookmarked(userInfo.uid);
                    var user = await _livemeApiProvider.GetUserInfo(userInfo.uid, _cts.Token);
                    Users.Add(new ListViewUserCard
                    {
                        UserInfo = userInfo,
                        LiveMeUser = user,
                        LastSeen = lastSeen,
                        IsBookmarked = bookmarked
                    });
                    NofifyChanged();

                }
            } while (users.Count() == MaxPageSize && iterations < maxIndex);
        }

        private static string LastSeen(ProfileSeen watched)
        {
            string lastSeen = string.Empty;
            if (watched != null)
            {
                var delta = DateTime.UtcNow - watched.Seen.FromUnixTimestamp();
                if (delta.TotalHours < 1)
                    lastSeen = "Just now";
                else if (delta.TotalDays < 1)
                    lastSeen = delta.TotalHours.ToString("N1") + " hours ago";
                else
                    lastSeen = delta.TotalDays.ToString("N1") + " days ago";
            }

            return lastSeen;
        }

        public void ShowUser(UserInfo userInfo)
        {
            var uid = userInfo.uid;


            Users.First(x => x.UserInfo?.uid == uid).LastSeen = "Just now";

            _viewModelMediator.Send(ViewModelNotification.ShowUser, uid);
        }

        public void AddToBookmark(LiveMeUser user)
        {
            _dataAccess.AddToBookmark(user);
        }
    }
}