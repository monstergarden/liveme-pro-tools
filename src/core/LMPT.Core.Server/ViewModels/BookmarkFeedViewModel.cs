using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB;
using LMPT.Core.Server.Shared;
using LMPT.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Server.ViewModels
{
    public class BookmarkFeedViewModel : BaseViewModel, ISideBarViewModel
    {
        public string SvgIconPath  =>  Icons.People;
        public string Title => "Bookmark Feeds";

        private readonly BookmarkScanner _bookmarkScanner;
        private readonly ILogger<BookmarkFeedViewModel> _logger;
        private readonly MainViewModel mainViewModel;

        public BookmarkFeedViewModel(
            ILogger<BookmarkFeedViewModel> logger,
            MainViewModel mainViewModel,
            BookmarkScanner bookmarkScanner)
        {
            _logger = logger;
            this.mainViewModel = mainViewModel;
            _bookmarkScanner = bookmarkScanner;

            BookmarkFeed = new ObservableCollection<ScanResult>();
            BookmarkFeed.CollectionChanged += (s, e) => NofifyChanged("BookmarkFeed");
            BookmarkFeedButtonText = "Scan";
            BookmarkFeedButtonDisabled = false;
        }

        public string BookmarkFeedButtonText { get; set; }
        public bool BookmarkFeedButtonDisabled { get; set; }
        public ObservableCollection<ScanResult> BookmarkFeed { get; set; }
        public IEnumerable<ScanResult> NewReplays => BookmarkFeed.Where(x => x.ScanType == ScanType.NewReplays);
        public IEnumerable<ScanResult> NewFans => BookmarkFeed.Where(x => x.ScanType == ScanType.NewFans);
        public IEnumerable<ScanResult> NewFollowings => BookmarkFeed.Where(x => x.ScanType == ScanType.NewFollowings);

        public void Open()
        {
            mainViewModel.OpenBookmarkFeeds();
        }



        public async Task LoadLastScanResults()
        {
            // _footerViewModel.FooterInfo = "Restoring last scan ...";
            var lastScan = _bookmarkScanner.LoadLastScanResults();
            await ResetAndAddtoObser(BookmarkFeed, lastScan);
            // _footerViewModel.FooterInfo = "Restored last scan result.";
        }


        public async Task Rescan()
        {
            try
            {
                BookmarkFeedButtonText = "Scanning ...";
                BookmarkFeedButtonDisabled = true;
                BookmarkFeed.Clear();


                await _bookmarkScanner.Rescan(
                    result => BookmarkFeed.Add(result),
                    (x) => {});

                // _footerViewModel.DisplayProgressBar = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                BookmarkFeedButtonText = "Rescan";
                BookmarkFeedButtonDisabled = false;
            }
        }


        private async Task ResetAndAddtoObser<T>(ObservableCollection<T> collection, IEnumerable<T> range)
        {
            try
            {
                collection.Clear();
                foreach (var item in range)
                {
                    await Task.Delay(10).ConfigureAwait(true);
                    collection.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}