namespace LMPT.Core.Contract.DB
{
    public class LivemeAuthentication
    {
//        [Key]
        public int ID { get; set; } = 1;
        public string SsoToken { get; set; }
        public string Token { get; set; }
        public string Sid { get; set; }
        public string Tuid { get; set; }
        public long LoginTimestamp { get; set; }
    }
}