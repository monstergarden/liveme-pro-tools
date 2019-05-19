using Newtonsoft.Json;

namespace LMPT.Core.Contract.LegacyJson
{
    public class BookmarkJson
    {
        [JsonProperty("counts")] public Counts Counts { get; set; }

        [JsonProperty("lastViewed")] public long LastViewed { get; set; }

        [JsonProperty("newestReplay")] public long NewestReplay { get; set; }

        [JsonProperty("uid")] public string Uid { get; set; }

        [JsonProperty("shortid")] public long Shortid { get; set; }

        [JsonProperty("signature")] public string Signature { get; set; }

        [JsonProperty("sex")] public string Sex { get; set; }

        [JsonProperty("face")] public string Face { get; set; }

        [JsonProperty("nickname")] public string Nickname { get; set; }

        [JsonProperty("lamd")] public Lamd Lamd { get; set; }

        [JsonProperty("newest_replay")] public long BookmarkLegacyNewestReplay => NewestReplay;

        [JsonProperty("last_viewed")] public long BookmarkLegacyLastViewed => LastViewed;
    }

    public class Counts
    {
        [JsonProperty("replays")] public long Replays { get; set; }

        [JsonProperty("friends")] public long Friends { get; set; }

        [JsonProperty("followers")] public long Followers { get; set; }

        [JsonProperty("followings")] public long Followings { get; set; }

        [JsonProperty("changed")] public bool Changed { get; set; }
    }

    public class Lamd
    {
        [JsonProperty("monitor")] public bool Monitor { get; set; }

        [JsonProperty("last_checked")] public long LastChecked { get; set; }
    }
}