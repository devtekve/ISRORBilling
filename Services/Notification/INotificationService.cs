using ISRORBilling.Models.Notification;

namespace ISRORBilling.Services.Notification;

public interface INotificationService
{
    Task<bool> SendSecondPassword(SendCodeRequest request);
    Task<bool> SendItemLockCode(SendCodeRequest request);
}