using System.Collections.Generic;

namespace UptimeBoard.Node.Models
{
    public class DeviceRequest
    {
        public List<DeviceConfig> Devices { get; set; }
        public string ResultApi { get; set; }
        public int RequestInterval { get; set; } = 30000;
    }
}