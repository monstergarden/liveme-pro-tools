using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB;
using LMPT.Core.Contract.DB.Bookmark;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.LivemeApi;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Services
{
    public class BookmarkScanner
    {
        private readonly DataAccess _db;


        private readonly LivemeApiProvider _liveMeApi;
        private readonly ILogger<BookmarkScanner> _logger;


        public BookmarkScanner(
            ILogger<BookmarkScanner> logger,
            DataAccess dataAccess,
            LivemeApiProvider lmProvider)
        {
            _logger = logger;
            _db = dataAccess;
            _liveMeApi = lmProvider;
        }

        public int Count { get; set; }
        public double TotalCount { get; set; }

        public async Task Rescan(Action<ScanResult> onScanResult, Action<double> progress)
        {
            var allBookmarks = _db.GetAllBookmarks().ToList();
            allBookmarks.Reverse();

            var tasks = new List<Task>();
            var results = new List<ScanResult>();

            Count = 0;
            TotalCount = allBookmarks.Count;

            foreach (var bookmark in allBookmarks)
            {
                var task = ScanBookmark(bookmark, s =>
                {
                    results.Add(s);
                    onScanResult(s);
                }, progress);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            StoreScan(results);
        }

        private async Task ScanBookmark(Bookmark bookmark, Action<ScanResult> onScanFound, Action<double> progress)
        {
            var liveMeUser = await _liveMeApi.GetUserInfo(bookmark.Uid, CancellationToken.None);

//            lock (_footerViewModel)
//            {
//                Count++;
//                progress(Count / TotalCount);
//                _footerViewModel.FooterInfo = $"Scanning {Count} / {TotalCount} bookmarks";
//            }

            //await JsInteropHelper.CallAndGet<object>("scanFooterInfo", current, totalLength);

            var newReplays = bookmark.ReplayCount < liveMeUser.CountInfo.VideoCount;
            var newFans = bookmark.FollowerCount < liveMeUser.CountInfo.FollowerCount;
            var newFollowings = bookmark.FollowingCount < liveMeUser.CountInfo.FollowingCount;
            var showReplays = bookmark.BookmarkType == BookmarkType.Default ||
                              bookmark.BookmarkType == BookmarkType.Streamer;
            var showFans = bookmark.BookmarkType == BookmarkType.Default ||
                           bookmark.BookmarkType == BookmarkType.Streamer;
            var showFollowings = bookmark.BookmarkType == BookmarkType.Default ||
                                 bookmark.BookmarkType == BookmarkType.Follower;

            if (newReplays && showReplays)

            {
                var delta = (int) (liveMeUser.CountInfo.VideoCount - bookmark.ReplayCount);
                onScanFound(new ScanResult
                {
                    Delta = delta,
                    ScanType = ScanType.NewReplays,
                    Bookmark = bookmark
                });
            }

            if (newFollowings && showFollowings)
            {
                var delta = (int) (liveMeUser.CountInfo.FollowingCount - bookmark.FollowingCount);
                onScanFound(new ScanResult
                {
                    Delta = delta,
                    ScanType = ScanType.NewFollowings,
                    Bookmark = bookmark
                });
            }

            if (newFans && showFans)
            {
                var delta = (int) (liveMeUser.CountInfo.FollowerCount - bookmark.FollowerCount);
                onScanFound(new ScanResult
                {
                    Delta = delta,
                    ScanType = ScanType.NewFans,
                    Bookmark = bookmark
                });
            }

            if (newFans || newFollowings || newReplays)
                _db.UpdateBookmark(bookmark.Uid, liveMeUser);
        }


        private void StoreScan(IEnumerable<ScanResult> results)
        {
            _db.ReplaceLastScanResult(results);
        }

        public IEnumerable<ScanResult> LoadLastScanResults()
        {
            _logger.LogInformation("Loading last bookmark feed.");
            return _db.LoadLastScanResults();
        }
    }
}