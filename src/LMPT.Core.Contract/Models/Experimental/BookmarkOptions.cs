using System;
using System.Collections.Generic;

namespace LMPT.Core.Contract.Models.Experimental
{
    public class BookmarkOptions
    {
        public string Uid { get; set; }
        public BookmarkType BookmarkType { get; set; }
        public List<Tag> Tags { get; set; }
        public ScanGroup ScanGroup { get; set; }

        // Minimal time it will not call liveme and instead use cached result.
        public TimeSpan TimeToUseCache { get; set; }
    }

    public class ScanGroup
    {
        public int _id { get; set; }

        public List<Scan> ScanType { get; set; }

        // Minimal time a background process or bookmark scan will wait before doing new.
        public TimeSpan RescanAfter { get; set; }
    }

    public class Tag
    {
        public Guid TagId { get; set; }
        public string Color { get; set; }
        public string TagName { get; set; }
    }

    public enum Scan
    {
        Fans,
        Followings,
        Replays
    }
}