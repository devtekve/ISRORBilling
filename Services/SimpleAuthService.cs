using ISRORBilling.Database;
using ISRORBilling.Models;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling.Services;

public class SimpleAuthService : IAuthService
{
    private readonly AccountContext _accountContext;

    public SimpleAuthService(AccountContext accountContext) => _accountContext = accountContext;


    public AUserLoginResponse Login(string userId, string userPw, string channel)
    {
        if (userId.IsNullOrEmpty()) return new AUserLoginResponse() {ReturnValue = LoginResponseCodeEnum.Error};
        var user = channel switch
        {
            "1" => _accountContext.Users.FirstOrDefault(user => user.StrUserID == userId && user.password == userPw),
            // "2" => _accountContext.Users.FirstOrDefault(user => user.StrEmail == userId && user.passwordSha256 == userPw),
            _ => null
        };

        if (user == null) return new AUserLoginResponse() {ReturnValue = LoginResponseCodeEnum.Error};

        return new AUserLoginResponse
        {
            ReturnValue = LoginResponseCodeEnum.Success,
            JID = user.PortalJID,
            // CurrentDate = null,
            ARCode = null,
            EmailAddr = user.StrEmail,
            EmailCertificationStatus = null,
            EmailUniqueStatus = null,
            NickName = null,
            VipLevel = null,
            VipExpireTime = null,
            VipUserType = null
        };
    }
}