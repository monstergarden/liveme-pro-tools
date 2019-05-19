using System;
using Newtonsoft.Json;

namespace LMPT.Core.Contract.FromLiveMe
{
    public class ReplayFromLiveme
    {
        [JsonProperty("vid")] public string Vid { get; set; }

        [JsonProperty("watchnumber")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Watchnumber { get; set; }

        [JsonProperty("topicid")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Topicid { get; set; }

        [JsonProperty("subtype")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Subtype { get; set; }

        [JsonProperty("jointype")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Jointype { get; set; }

        [JsonProperty("topic")] public string Topic { get; set; }

        [JsonProperty("vtime")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Vtime { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("userid")] public string Userid { get; set; }

        [JsonProperty("online")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Online { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Status { get; set; }

        [JsonProperty("likenum")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Likenum { get; set; }

        [JsonProperty("playnumber")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Playnumber { get; set; }

        [JsonProperty("roomstate")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Roomstate { get; set; }

        [JsonProperty("videosize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Videosize { get; set; }

        [JsonProperty("videolength")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Videolength { get; set; }

        [JsonProperty("server_agent")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ServerAgent { get; set; }

        [JsonProperty("videosource")] public Uri Videosource { get; set; }

        [JsonProperty("smallsource")] public string Smallsource { get; set; }

        [JsonProperty("smallsourcemore")] public string Smallsourcemore { get; set; }

        [JsonProperty("videosourcemore")] public string Videosourcemore { get; set; }

        [JsonProperty("videos_flv")] public string VideosFlv { get; set; }

        [JsonProperty("videos_hls")] public string VideosHls { get; set; }

        [JsonProperty("hlsvideosource")] public Uri Hlsvideosource { get; set; }

        [JsonProperty("sdkstreamid")] public string Sdkstreamid { get; set; }

        [JsonProperty("videocapture")] public Uri Videocapture { get; set; }

        [JsonProperty("rotate")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Rotate { get; set; }

        [JsonProperty("msgfile")] public Uri Msgfile { get; set; }

        [JsonProperty("gzip_msgfile")] public Uri GzipMsgfile { get; set; }

        [JsonProperty("vdoid")] public string Vdoid { get; set; }

        [JsonProperty("sharenum")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Sharenum { get; set; }

        [JsonProperty("shareuv")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Shareuv { get; set; }

        [JsonProperty("istop")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Istop { get; set; }

        [JsonProperty("shareurl")] public Uri Shareurl { get; set; }

        [JsonProperty("addr")] public string Addr { get; set; }

        [JsonProperty("isaddr")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Isaddr { get; set; }

        [JsonProperty("lnt")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Lnt { get; set; }

        [JsonProperty("lat")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Lat { get; set; }

        [JsonProperty("area")] public string Area { get; set; }

        [JsonProperty("countryCode")] public string CountryCode { get; set; }

        [JsonProperty("chatSystem")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ChatSystem { get; set; }

        [JsonProperty("gscreen")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Gscreen { get; set; }

        [JsonProperty("vtype")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Vtype { get; set; }

        [JsonProperty("isTCLine")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IsTcLine { get; set; }

        [JsonProperty("TCRoomId")] public string TcRoomId { get; set; }

        [JsonProperty("supportLine")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long SupportLine { get; set; }

        [JsonProperty("announcement_shop")] public string AnnouncementShop { get; set; }

        [JsonProperty("charades")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Charades { get; set; }

        [JsonProperty("trivia")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Trivia { get; set; }

        [JsonProperty("ispvt")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Ispvt { get; set; }

        [JsonProperty("homepage")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Homepage { get; set; }

        [JsonProperty("is_shelf")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IsShelf { get; set; }

        [JsonProperty("heat")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Heat { get; set; }

        [JsonProperty("uface")] public Uri Uface { get; set; }

        [JsonProperty("expire_time")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ExpireTime { get; set; }

        [JsonProperty("level")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Level { get; set; }

        [JsonProperty("sex")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Sex { get; set; }

        [JsonProperty("uname")] public string Uname { get; set; }

        [JsonProperty("is_verified")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IsVerified { get; set; }

        [JsonProperty("smallcover")] public Uri Smallcover { get; set; }
    }
}