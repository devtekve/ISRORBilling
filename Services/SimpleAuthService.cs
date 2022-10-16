using ISRORBilling.Database;
using ISRORBilling.Models;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling.Services;

public class SimpleAuthService : IAuthService
{
    private readonly AccountContext _accountContext;
    private readonly ILogger<SimpleAuthService> _logger;

    public SimpleAuthService(AccountContext accountContext, ILogger<SimpleAuthService> logger)
    {
        _accountContext = accountContext;
        _logger = logger;
    }

    public AUserLoginResponse Login(CheckUserRequest request)
    {
        if (!request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return new AUserLoginResponse { ReturnValue = LoginResponseCodeEnum.Emergency };
        }

        return Login(request.UserId, request.HashedUserPassword, request.ChannelId.ToString());
    }
    public AUserLoginResponse Login(string userId, string userPw, string channel)
    {
        if (userId.IsNullOrEmpty()) 
            return new AUserLoginResponse() {ReturnValue = LoginResponseCodeEnum.Error};
        
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