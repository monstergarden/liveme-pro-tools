using System;
using Newtonsoft.Json;

namespace LMPT.Core.Contract.DB.Bookmark
{
    public class Bookmark
    {
        [JsonProperty("uid")] public string Uid { get; set; }

        [JsonProperty("shortid")] public long Shortid { get; set; }

        [JsonProperty("bookmarkType")] public BookmarkType BookmarkType { get; set; }

        [JsonProperty("replayCount")] public long ReplayCount { get; set; }

        [JsonProperty("followerCount")] public long FollowerCount { get; set; }

        [JsonProperty("followingCount")] public long FollowingCount { get; set; }

        [JsonProperty("signature")] public string Signature { get; set; }

        [JsonProperty("gender")] public Gender Gender { get; set; }

        [JsonProperty("face")] public string Face { get; set; }

        [JsonProperty("nickname")] public string Nickname { get; set; }

        [JsonProperty("lastUpdated")] public DateTime LastUpdated { get; set; }

        [JsonProperty("deleted")] public bool Deleted { get; set; }
    }
}