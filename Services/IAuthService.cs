using ISRORBilling.Models;

namespace ISRORBilling.Services;

public interface IAuthService
{
    AUserLoginResponse Login(string userId, string userPw, string channel);
}