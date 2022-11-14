using System.Security.Cryptography;
using System.Text;

namespace ISRORBilling.Models.Notification;

/// <summary>
/// Exactly the same format seems to be used for both Item Lock and Sending Secondary Password by email.
/// </summary>
public class SendCodeRequest : GatewayRequest
{
    public int jid { get; }
    public string code { get; }
    public string email { get; }

    protected override string CalculatedToken
    {
        get
        {
            using var md5 = MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(
                Encoding.ASCII.GetBytes($"{jid}{code}{email}{SaltKey}")));
        }
    }

    public SendCodeRequest(string values, string? saltKey = null)
    {
        SaltKey = saltKey;
        var allValues = values.Split('|');
        jid = int.Parse(allValues[0]);
        code = allValues[1];
        email = allValues[2];
        UserProvidedValidationToken = allValues.ElementAtOrDefault(3);
    }

}