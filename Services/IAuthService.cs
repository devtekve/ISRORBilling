using ISRORBilling.Models;

namespace ISRORBilling.Services;

public interface IAuthService
{
    AUserLoginResponse Login(string userId, string userPw, string channel) =>
        Login(new CheckUserRequest($"{channel}|{userId}|{userPw}"));

    AUserLoginResponse Login(CheckUserRequest request) =>
        Login(request.UserId, request.HashedUserPassword, request.ChannelId.ToString());
}