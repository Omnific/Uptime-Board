using System.Collections.Generic;

namespace UptimeBoard.RequestServer.Models
{
    public class DeviceRequest
    {
        public List<DeviceViewModel> Devices { get; set; }
        public string ResultApi { get; set; }
        public int RequestInterval { get; set; } = 30000;
    }

    public class DeviceViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int RequestTimeout { get; set; }
        public RequestType Type { get; set; }
        public int Total { get; set; } = 1;
        public int PacketByteSize { get; set; } = 32;
    }

    public enum RequestType : int
    {
        Ping = 0,
        Get = 1,
        Post = 2
    }
}