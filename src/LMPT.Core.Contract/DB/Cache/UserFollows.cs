namespace LMPT.Core.Contract.DB.Cache
{
    public class UserFollows
    {
        public Bookmark.Bookmark Bookmark { get; set; }
        public string FollowingUserId { get; set; }
    }
}