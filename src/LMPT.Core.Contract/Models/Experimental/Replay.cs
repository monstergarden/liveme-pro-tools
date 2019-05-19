using System;
using LMPT.Core.Contract.FromLiveMe;

namespace LMPT.Core.Contract.Models.Experimental
{
    public class Replay
    {
        public User FromUser { get; set; }
        public Uri VideoUrl { get; set; }
        public string Title { get; set; }
        public bool Watched { get; set; }
        public TimeSpan Duration { get; set; }

        public long ViewCount { get; set; }
        public long ShareCount { get; set; }
        public long LikeCount { get; set; }

        public float VPM { get; set; }
        public float LMP { get; set; }
        public float SPM { get; set; }
    }
}