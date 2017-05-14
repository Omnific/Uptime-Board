namespace UptimeBoard.Node.Models
{
    public class DeviceResponse
    {
        public string Name { get; set; }
        public bool Success { get; set; }
        public long TotalMs { get; set; }
    }
}