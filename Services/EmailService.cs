using Azure.Core;
using ISRORBilling.Models;

namespace ISRORBilling.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger) => _logger = logger;

    public Task<bool> SendSecondPasswordByEmail(SendSecondPasswordByEmailRequest request)
    {
        if (!request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return Task.FromResult(false);
        }

        _logger.LogError($"Sending Secundary Password by email is not implemented! Failed for [{request.email}]");
        return Task.FromResult(false);
    }

    public Task<bool> SendItemCodeByEmail(SendItemLockByEmailRequest request)
    {
        if (!request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return Task.FromResult(false);
        }

        _logger.LogError($"Sending Item Lock Key by email is not implemented! Failed for [{request.email}]");
        return Task.FromResult(false);
    }
}