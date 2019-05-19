using System;

namespace LMPT.Core.Contract.DB.Cache
{
    public class Replay
    {
        public string VideoUrl { get; set; }
        public string Title { get; set; }
        public User FromUser { get; set; }
        public bool Watched { get; set; }
        public TimeSpan Duration { get; set; }
        public string VId { get; set; }
        public string Url { get; set; }
        public long StartTimeStamp { get; set; }
        public long ShareNum { get; set; }
        public long CreatedAt { get; set; }
        public bool Downloaded { get; set; }
        public bool Liked { get; set; }
    }
}