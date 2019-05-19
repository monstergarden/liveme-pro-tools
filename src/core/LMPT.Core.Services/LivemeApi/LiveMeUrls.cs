namespace LMPT.Core.Services.LivemeApi
{
    internal static class LiveMeUrls
    {
        private const string Api = "https://live.ksmobile.net";
        private const string Iag = "https://iag.ksmobile.net";
        public static readonly string Login = $"{Iag}/1/cgi/login";
        public static readonly string AppLogin = $"{Api}/sns/appLoginCM";
        public static string UrlaccessToken = $"{Api}/channel/signin";
        public static string ChannelLogin = $"{Api}/channel/login";
        public static readonly string UserInfo = $"{Api}/user/getinfo";
        public static string VideoInfo = $"{Api}/live/queryinfo";
        public static readonly string ReplayVideos = $"{Api}/live/getreplayvideos";
        public static string KeywordSearch = $"{Api}/search/searchkeyword";
        public static string LiveUsers = $"{Api}/live/newmaininfo";
        public static string Fans = $"{Api}/follow/getfollowerlistship";
        public static string Following = $"{Api}/follow/getfollowinglistship";
        public static string TrendingHashtags = $"{Api}/search/getTags";
        public static string LiveBoys = $"{Api}/live/boys";
        public static string LiveGirls = $"{Api}/live/girls";
    }
}