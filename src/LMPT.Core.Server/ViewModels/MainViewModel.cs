using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB.Cache;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.LivemeApi;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Server.ViewModels
{
    public enum SearchType
    {
        [Display(Name = "Video ID is")] VideoID,
        [Display(Name = "Video URL is")] videoUrl,
        [Display(Name = "User ID is")] userID,
        [Display(Name = "User Short ID is")] userShortID,
        [Display(Name = "Username like")] usernameLike,
        [Display(Name = "Hastag like")] hastagLike
    }

    public class MainViewModel : BaseViewModel, IDisposable
    {
        private readonly DataAccess _dataAccess;
        private readonly LivemeApiProvider _livemeApi;
        private readonly ILogger<MainViewModel> _logger;
        private readonly ProfileViewModel _profileViewModel;
        private readonly ViewModelMediator _viewModelMediator;
        private readonly SidebarViewModelFactory _sidebarFactory;
        public ISideBarViewModel BookmarkVm => _sidebarFactory.CreateBookmarksViewModel();
        public ISideBarViewModel BookmarkFeedVm => _sidebarFactory.CreateBookmarksFeedViewModel();
        public ListWindowViewModel ListVm => _sidebarFactory.CreateListWindowViewModel();
        public MainViews CurrentView { get; set; }
        public IEnumerable<User> LastVisited { get; set; }

        public string SearchValue { get; set; } = string.Empty;
        public SearchType SearchType { get; set; }


        public ISideBarViewModel SidebarViewModel { get; set; }
        private CancellationTokenSource _tokenSource;



        public MainViewModel(ILogger<MainViewModel> logger,
            DataAccess dataAccess,
            ViewModelMediator viewModelMediator,
            SidebarViewModelFactory sidebarFactory,
            ProfileViewModel profileViewModel,
            LivemeApiProvider api
        )
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _viewModelMediator = viewModelMediator;
            _sidebarFactory = sidebarFactory;
            _profileViewModel = profileViewModel;
            _livemeApi = api;
            _viewModelMediator.OnNotfication += DispatchNotification;

            LastVisited = new List<User>();
            SearchType = SearchType.userID;
        }

  

        


        public void Dispose()
        {
            _viewModelMediator.OnNotfication -= DispatchNotification;
        }

        private async void DispatchNotification(ViewModelNotification type, object arg)
        {
            if (type == ViewModelNotification.ShowUser) await ShowUser((string)arg);
        }

        public void OpenBrowserGithub()
        {
            Services.Helper.Utils.OpenBrowser(@"https://github.com/thecoder75/liveme-pro-tools");
        }

        public void OpenBookmarks()
        {
            SidebarViewModel = BookmarkVm;
            NofifyChanged();
        }
        public void OpenBookmarkFeeds()
        {
            SidebarViewModel = BookmarkFeedVm;
            NofifyChanged();
        }

        public void OpenListWindow()
        {
            SidebarViewModel = ListVm;
            NofifyChanged();
        }

        public void OpenFans(string uid)
        {
            SidebarViewModel = ListVm;
            ListVm.ShowFansOf(uid);
            NofifyChanged();
        }

        public void OpenFollowers(string uid)
        {

            SidebarViewModel = ListVm;
            ListVm.ShowFollowingsOf(uid);

            NofifyChanged();
        }

        public void QuitServer()
        {
            Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                Environment.Exit(-1);
            });
        }


        public async Task Search()
        {
            try
            {
                switch (SearchType)
                {
                    case SearchType.VideoID:
                        var video = await _livemeApi.GetVideoInfo(SearchValue, CancellationToken.None);
                        await ShowUser(video.Userid);
                        break;
                    case SearchType.userID:
                        await ShowUser(SearchValue);
                        break;
                    default:
                        CurrentView = MainViews.WorkInProgress;
                        break;
                }
            }
            catch (Exception)
            {
                CurrentView = MainViews.NotFound;
            }

            NofifyChanged();
        }

        public async Task ShowUser(string uid)
        {
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();

            CurrentView = MainViews.Profile;
            SearchValue = uid;
            SearchType = SearchType.userID;

            NofifyChanged();

            await _profileViewModel.LoadUser(uid, _tokenSource.Token);
        }


        public async Task Init()
        {
            await _dataAccess.MigrateAsync().ConfigureAwait(true);
            FetchLastProfilesVisited();
        }


        private void FetchLastProfilesVisited()
        {
            try
            {
                LastVisited = _dataAccess.GetLastProfilesVisited();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }


        public void GoHome()
        {
            CurrentView = MainViews.Home;
            NofifyChanged();
            FetchLastProfilesVisited();
        }
    }

    public enum MainViews
    {
        Home,
        Profile,
        WorkInProgress,
        NotFound,
        Settings
    }

    public enum MainViewStatus
    {
        LoadingProfile,
        ProfileNotFound,
        LoadingReplays,
        NoReplaysFound,
        ReplaysFound
    }
}