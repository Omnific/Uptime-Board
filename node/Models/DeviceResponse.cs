namespace UptimeBoard.Node.Models
{
    public class DeviceResponse
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Success { get; set; }
        public long Total { get; set; }
        public long TotalMs { get; set; }
        public bool Up { get; set; }
    }
}