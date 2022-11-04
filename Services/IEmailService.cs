namespace ISRORBilling.Services;

public interface IEmailService
{
    Task<bool> SendSecondPasswordByEmail(string newCode, string strEmail);
    Task<bool> SendItemCodeByEmail(string lockCode, string strEmail);
}