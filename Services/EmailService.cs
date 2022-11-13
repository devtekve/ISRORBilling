using ISRORBilling.Models;

namespace ISRORBilling.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger) => _logger = logger;

    public Task<bool> SendLockItemKeyByEmail(SendItemLockByEmailRequest request)
    {
        if (!request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return Task.FromResult(false);
        }

        _logger.LogError($"Sending Item Lock Key by email is not implemented! Failed for [{request.email}]");
        return Task.FromResult(false);

    }

    public Task<bool> SendSecondPasswordByEmail(string newCode, string strEmail)
    {
        _logger.LogError($"Sending second password by email is not implemented! Failed for [{strEmail}]");
        return Task.FromResult(false);
    }
}