using System.Collections.Generic;
using LMPT.Core.Contract.FromLiveMe;

namespace LMPT.Core.Contract.Models.Experimental
{
    public class PlayListEntry
    {
        public User User { get; set; }
        public List<Replay> ReplaysSubset { get; set; }
        public float Variance { get; set; }
    }
}