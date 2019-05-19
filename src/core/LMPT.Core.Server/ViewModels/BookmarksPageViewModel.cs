using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB.Bookmark;
using LMPT.Core.Server.Shared;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.Helper;

namespace LMPT.Core.Server.ViewModels
{
    public class BookmarksPageViewModel : BaseViewModel, ISideBarViewModel
    {
        public string SvgIconPath => Icons.Bookmark;
        public string Title => "Bookmarks";

        private readonly DataAccess _db;
        private readonly MainViewModel mainViewModel;
        private IEnumerable<Bookmark> _bookmarksSource = new List<Bookmark>();

        public BookmarksPageViewModel(DataAccess db, MainViewModel mainViewModel)
        {
            _db = db;
            this.mainViewModel = mainViewModel;
            Bookmarks = new ObservableCollection<Bookmark>();
            Bookmarks.CollectionChanged += (e, s) => NofifyChanged("Bookmarks");
        }

        public void Open()
        {
            mainViewModel.OpenBookmarks();
        }

        public ObservableCollection<Bookmark> Bookmarks { get; set; }
        public string FilterName { get; set; }
        public int? FilterDaysActive { get; set; }


        public async Task InitAsync()
        {
            _bookmarksSource = _db.GetAllBookmarks();
            await ApplyFilters();
        }


        public async Task ApplyFilters()
        {
            Bookmarks.Clear();
            var filtered = _bookmarksSource
                .Where(FilterByLastActive)
                .Where(FilterByName);

            foreach (var b in filtered)
            {
                await Task.Delay(10).ConfigureAwait(false);
                Bookmarks.Add(b);
            }
        }

        private bool FilterByName(Bookmark b)
        {
            if (string.IsNullOrWhiteSpace(FilterName)) return true;
            return b.Nickname.Contains(FilterName, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool FilterByLastActive(Bookmark b)
        {
            if (FilterDaysActive == null)
                return true;

            var last = _db.GetLastReplaysFromUid(b.Uid, 1).FirstOrDefault();
            if (last != null)
            {
                var delta = DateTime.Now - last.StartTimeStamp.FromUnixTimestamp();
                return delta.TotalDays < FilterDaysActive;
            }

            return false;
        }
    }
}