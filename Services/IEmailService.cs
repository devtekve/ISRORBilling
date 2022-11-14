using ISRORBilling.Models;

namespace ISRORBilling.Services;

public interface IEmailService
{
    Task<bool> SendSecondPasswordByEmail(SendSecondPasswordByEmailRequest request);
    Task<bool> SendItemCodeByEmail(SendItemLockByEmailRequest request);
}