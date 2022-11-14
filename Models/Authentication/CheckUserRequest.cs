using System.Security.Cryptography;
using System.Text;

namespace ISRORBilling.Models.Authentication;

public class CheckUserRequest : GatewayRequest
{
    public short ChannelId { get; }
    public string UserId { get; }
    public string HashedUserPassword { get; }
    public string UserIp { get; }
    public int UnixTimeStamp { get; }
    protected override string CalculatedToken
    {
        get
        {
            using var md5 = MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(
                Encoding.ASCII.GetBytes($"{ChannelId}{UserId}{HashedUserPassword}{UserIp}{UnixTimeStamp}{SaltKey}")));
        }
    }

    public CheckUserRequest(string values, string? saltKey = null)
    {
        SaltKey = saltKey;
        var allValues = values.Split('|');
        ChannelId = short.Parse(allValues[0]);
        UserId = allValues[1];
        HashedUserPassword = allValues[2];
        UserIp = allValues.ElementAtOrDefault(3) ?? "0";
        UnixTimeStamp = int.Parse(allValues.ElementAtOrDefault(4) ?? "0");
        UserProvidedValidationToken = allValues.ElementAtOrDefault(5);
    }
}