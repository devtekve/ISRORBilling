using System.Net;

namespace ISRORBilling.Models.Ping;

public class NationPingServiceOptions
{
    public string ListenAddress { get; set; } = $"{IPAddress.Loopback}";
    public int ListenPort { get; set; } = 12989;
}