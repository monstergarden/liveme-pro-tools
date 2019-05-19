using System;
using Newtonsoft.Json;

namespace LMPT.Core.Contract.FromLiveMe
{
    public class User
    {
        [JsonProperty("uid")] public string Uid { get; set; }

        [JsonProperty("shortid")] public long Shortid { get; set; }

        [JsonProperty("signature")] public string Signature { get; set; }

        [JsonProperty("sex")] public string Sex { get; set; }

        [JsonProperty("face")] public Uri Face { get; set; }

        [JsonProperty("nickname")] public string Nickname { get; set; }
    }
}