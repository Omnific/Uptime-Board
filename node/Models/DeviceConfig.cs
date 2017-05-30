namespace UptimeBoard.Node.Models
{
    public class DeviceConfig
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int RequestTimeout { get; set; }
        public RequestType Type { get; set; }
        public int Total { get; set; } = 1;
    }

    public enum RequestType : int
    {
        Ping = 0,
        Get = 1,
        Post = 2
    }
}