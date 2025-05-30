namespace HoleAutoJoin.Core
{
    public class JoinResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int JoinedCount { get; set; }
        public int FailedCount { get; set; }
    }
}