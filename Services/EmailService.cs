namespace ISRORBilling.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger) => _logger = logger;

    public Task<bool> SendSecondPasswordByEmail(string newCode, string strEmail)
    {
        _logger.LogError($"Sending second password by email is not implemented! Failed for [{strEmail}]");
        return Task.FromResult(false);
    }
}