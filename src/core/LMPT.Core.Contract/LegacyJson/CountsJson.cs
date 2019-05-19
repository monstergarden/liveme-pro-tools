using Newtonsoft.Json;

namespace LMPT.Core.Contract.LegacyJson
{
    public class CountsJson
    {
        [JsonProperty("replays")] public long Replays { get; set; }

        [JsonProperty("friends")] public long Friends { get; set; }

        [JsonProperty("followers")] public long Followers { get; set; }

        [JsonProperty("followings")] public long Followings { get; set; }
    }
}