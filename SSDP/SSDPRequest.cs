namespace RMSUdpService.SSDP;

public class SSDPRequest
{
    public string Method { get; set; } = "";
    public Dictionary<string, string> Headers { get; set; }

    public SSDPRequest()
    {
        Headers = new Dictionary<string, string>();
    }
}
