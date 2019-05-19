namespace LMPT.Core.Contract.Models.Experimental
{
    public class UserOptions
    {
        public UserIgnore IgnoreForever { get; set; }
    }

    public enum UserIgnore
    {
        None = 0,
        Forever = 1,
        Session = 2
    }
}