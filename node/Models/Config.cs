namespace UptimeBoard.Node.Models
{
    public class Config
    {
        public string NodeName { get; set; }
        public string RequestApi { get; set; }
        public string ResultApi { get; set; }
        public int RequestInterval { get; set; }
    }
}