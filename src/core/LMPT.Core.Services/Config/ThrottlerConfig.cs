namespace LMPT.Core.Services.Config
{
    public class ThrottlerConfig
    {
        public Range RandomDelay { get; set; }
        public int MaxConcurrentCalls { get; set; }
    }

    public struct Range
    {
        public int Lower { get; set; }
        public int Upper { get; set; }

        public Range(int lower, int upper)
        {
            Lower = lower;
            Upper = upper;
        }
    }
}