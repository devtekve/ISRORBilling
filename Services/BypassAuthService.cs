using ISRORBilling.Database;
using ISRORBilling.Models;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling.Services;

/// <summary>
/// This should ONLY be used when developing, as it will not really check for the PW
/// </summary>
public class BypassAuthService : IAuthService
{
    private readonly AccountContext _accountContext;

    public BypassAuthService(AccountContext accountContext) => _accountContext = accountContext;


    public AUserLoginResponse Login(string userId, string userPw, string channel)
    {
        if (userId.IsNullOrEmpty()) return new AUserLoginResponse() {ReturnValue = LoginResponseCodeEnum.Error};
        var user = channel switch
        {
            "1" => _accountContext.Users.FirstOrDefault(user => user.StrUserID == userId ),
            _ => null
        };

        if (user == null) return new AUserLoginResponse() {ReturnValue = LoginResponseCodeEnum.NotFoundUid};

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