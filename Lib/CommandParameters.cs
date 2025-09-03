
namespace RMSUdpService.Lib
{
    public class CommandParameters
    {
        public string? RobotAddress { get; set; }
        public HttpClient? Client { get; set; }
        public string? BaseUrl { get; set; }
        public int Port { get; set; }
    }
}
