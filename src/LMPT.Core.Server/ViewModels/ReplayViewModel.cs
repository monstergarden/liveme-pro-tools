using System;
using System.Collections.Generic;
using System.Linq;
using LMPT.Core.Contract.DB.Cache;
using LMPT.Core.Contract.FromLiveMe;
using LMPT.Core.Services.Helper;

namespace LMPT.Core.Server.ViewModels
{
    public class ReplayViewModel : BaseViewModel
    {
        public ReplayViewModel(ReplayFromLiveme r, List<Replay> replayCache)
        {
            var duration = TimeSpan.FromSeconds(r.Videolength);

            string PerMin(double num, int digits)
            {
                return (num / duration.TotalMinutes).ToString($"n{digits}");
            }

            CheckAgainstCache(replayCache, r, out var watched, out var isnew, out var isLiked);
            SPM = PerMin(r.Sharenum, 1);
            LPM = PerMin(r.Likenum, 0);
            VPM = PerMin(r.Playnumber, 0);
            var tag = isnew ? "[NEW] " : "";
            Title = tag + " " + r.Title;
            EyeStyle = watched ? "bright green" : "dim";
            Date = r.Vtime.ToFormattedDateTime();
            Duration = TimeSpan.FromSeconds(r.Videolength).ToString();
            Likes = r.Likenum.ToString();
            Views = r.Playnumber.ToString();
            Shares = r.Sharenum.ToString();
            Vid = r.Vid;
            Watched = watched;
            New = isnew;
            IsLiked = isLiked;
            // IconsHtml = isLiked ?  : "";
        }

        public ReplayViewModel(Replay r)
        {
            string PerMin(double num, int digits)
            {
                return (num / r.Duration.TotalMinutes).ToString($"n{digits}");
            }

            SPM = PerMin(r.ShareNum, 1);
            LPM = "-";
            VPM = "-";
            var tag = "[Cache]";
            Title = tag + " " + r.Title;
            EyeStyle = r.Watched ? "bright green" : "dim";
            Date = r.StartTimeStamp.ToFormattedDateTime();
            Duration = r.Duration.ToString();
            Likes = "-";
            Views = "-";
            Shares = r.ShareNum.ToString();
            Vid = r.VId;
            Watched = Watched;
            New = false;
            IsLiked = IsLiked;
        }

        public string IconsHtml { get; set; }
        public bool Watched { get; set; }
        public bool New { get; set; }
        public bool IsLiked { get; }
        public string Title { get; set; } = "<null>";
        public string Likes { get; set; } = "<null>";
        public string Shares { get; set; } = "<null>";
        public string Vid { get; }
        public string Views { get; set; } = "<null>";
        public string Date { get; set; } = "<null>";
        public string Duration { get; set; } = "<null>";
        public string VPM { get; set; } = "<null>";
        public string LPM { get; set; } = "<null>";
        public string SPM { get; set; } = "<null>";
        public string EyeStyle { get; } = "<null>";

        private static void CheckAgainstCache(List<Replay> replayCache, ReplayFromLiveme r,
            out bool watched, out bool isNew, out bool isLiked)
        {
            var cache = replayCache.FirstOrDefault(c => c.VId == r.Vid);

            watched = cache?.Watched ?? false;
            isNew = CheckIsNew(cache);
            isLiked = cache?.Liked ?? false;
        }

        private static bool CheckIsNew(Replay cache)
        {
            var isNew = false;
            if (cache == null)
            {
                isNew = true;
            }
            else
            {
                var cacheTime = cache.CreatedAt.FromUnixTimestamp();
                var delta = DateTime.UtcNow - cacheTime;
                isNew = delta.TotalSeconds < 100;
            }

            return isNew;
        }
    }
}