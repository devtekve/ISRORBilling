using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling.Models;

public abstract class GatewayRequest
{
    /// <summary>
    /// This property should return the calculated token, which should match the received one on the request (UserProvidedValidationToken)
    /// </summary>
    protected abstract string CalculatedToken { get; }
    protected string? SaltKey { get; init; }
    protected string? UserProvidedValidationToken { get; init; }
    
    
    /// <summary>
    /// Compares the provided ValidationToken on the request with our own generated token. If no saltKey was provided, defaults to false.
    /// </summary>
    /// <returns></returns>
    public bool Validate() => !UserProvidedValidationToken.IsNullOrEmpty() && !SaltKey.IsNullOrEmpty() &&
                              UserProvidedValidationToken == CalculatedToken;

    public override string ToString() => JsonSerializer.Serialize(this);

}