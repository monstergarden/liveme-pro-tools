namespace LMPT.Core.Contract.DB
{
    public enum ScanType
    {
        NewReplays,
        NewFollowings,
        NewFans
    }

    public enum NewFeed
    {
        NewReplays,
        NewFollowings,
        NewFans
    }


    public class ScanResult
    {
        public int Id { get; set; }
        public int Delta { get; set; }
        public ScanType ScanType { get; set; }
        public Bookmark.Bookmark Bookmark { get; set; }
    }
}