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
    public class DownloadsViewModel : BaseViewModel, ISideBarViewModel
    {
        public string SvgIconPath  =>  "M17.896,12.706v-0.005v-0.003L15.855,2.507c-0.046-0.24-0.255-0.413-0.5-0.413H4.899c-0.24,0-0.447,0.166-0.498,0.4L2.106,12.696c-0.008,0.035-0.013,0.071-0.013,0.107v4.593c0,0.28,0.229,0.51,0.51,0.51h14.792c0.28,0,0.51-0.229,0.51-0.51v-4.593C17.906,12.77,17.904,12.737,17.896,12.706 M5.31,3.114h9.625l1.842,9.179h-4.481c-0.28,0-0.51,0.229-0.51,0.511c0,0.703-1.081,1.546-1.785,1.546c-0.704,0-1.785-0.843-1.785-1.546c0-0.281-0.229-0.511-0.51-0.511H3.239L5.31,3.114zM16.886,16.886H3.114v-3.572H7.25c0.235,1.021,1.658,2.032,2.75,2.032c1.092,0,2.515-1.012,2.749-2.032h4.137V16.886z";
        public string Title => "Downloads";

        private readonly BookmarkScanner _bookmarkScanner;
        private readonly ILogger<DownloadsViewModel> _logger;
        private readonly MainViewModel mainViewModel;

        public DownloadsViewModel(
            ILogger<DownloadsViewModel> logger,
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
            mainViewModel.SidebarViewModel = this;
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