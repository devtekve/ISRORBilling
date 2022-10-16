using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling.Models;

public class CheckUserRequest
{
    public readonly string? SaltKey;
    public short ChannelId { get; }
    public string UserId { get; }
    public string HashedUserPassword { get; }
    public string UserIp { get; }
    public int UnixTimeStamp { get; }
    public string? UserProvidedValidationToken { get; }

    public string CalculatedToken
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

    /// <summary>
    /// Compares the provided ValidationToken on the request with our own generated token. If no saltKey was provided, defaults to false.
    /// </summary>
    /// <returns></returns>
    public bool Validate()
    {
        return !UserProvidedValidationToken.IsNullOrEmpty() && !SaltKey.IsNullOrEmpty() &&
                     UserProvidedValidationToken == CalculatedToken;
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}