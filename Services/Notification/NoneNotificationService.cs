using ISRORBilling.Models.Notification;

namespace ISRORBilling.Services.Notification;

public class NoneNotificationService : INotificationService
{
    private readonly ILogger<NoneNotificationService> _logger;

    public NoneNotificationService(ILogger<NoneNotificationService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendSecondPassword(SendCodeRequest request)
    {
        _logger.LogWarning("Sending second password by email is not implemented! Failed for [{RequestEmail}]", request.email);
        return Task.FromResult(false);
    }

    public Task<bool> SendItemLockCode(SendCodeRequest request)
    {
        _logger.LogWarning("Sending Item Lock Key by email is not implemented! Failed for [{RequestEmail}]", request.email);
        return Task.FromResult(false);
    }
}