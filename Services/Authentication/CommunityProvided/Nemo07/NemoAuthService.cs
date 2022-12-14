using ISRORBilling.Database.CommunityProvided.Nemo07;
using ISRORBilling.Models.Authentication;
using ISRORBilling.Models.Enums;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling.Services.Authentication.CommunityProvided.Nemo07;

/// <summary>
/// This should ONLY be used when developing, as it will not really check for the PW
/// </summary>
public class NemoAuthService : IAuthService
{
    private readonly NemoAccountContext _accountContext;

    public NemoAuthService(NemoAccountContext accountContext) => _accountContext = accountContext;


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
            EmailAddr = user.Email,
            EmailCertificationStatus = user.EmailCertificationStatus,
            EmailUniqueStatus = user.EmailUniqueStatus,
            NickName = null,
            VipLevel = user.VIPLv,
            VipExpireTime = user.VipExpireTime,
            VipUserType = (VipUserType)user.VipUserType
        };
    }
}