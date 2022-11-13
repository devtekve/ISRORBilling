using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling.Models;

public class SendItemLockByEmailRequest
{
    public readonly string? SaltKey;
    public int jid { get; }
    public string code { get; }
    public string email { get; }
    public string? UserProvidedValidationToken { get; }
    public string CalculatedToken
    {
        get
        {
            using var md5 = MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(
                Encoding.ASCII.GetBytes($"{jid}{code}{email}{SaltKey}")));
        }
    }

    public SendItemLockByEmailRequest(string values, string? saltKey = null)
    {
        SaltKey = saltKey;
        var allValues = values.Split('|');
        jid = int.Parse(allValues[0]);
        code = allValues[1];
        email = allValues[2];
        UserProvidedValidationToken = allValues.ElementAtOrDefault(3);
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