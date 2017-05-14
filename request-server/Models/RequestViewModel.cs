namespace UptimeBoard.RequestServer.Models
{
    public class RequestViewModel
    {
        public string Address { get; set; }
        public int Threads { get; set; }
        public RequestType Type { get; set; }
    }

    public enum RequestType : int
    {
        Ping = 0,
        Get = 1,
        Post = 2
    }
}