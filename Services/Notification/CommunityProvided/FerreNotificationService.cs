using ISRORBilling.Database;
using ISRORBilling.Models.Authentication;
using ISRORBilling.Models.Notification;
using Microsoft.EntityFrameworkCore;

namespace ISRORBilling.Services.Notification.CommunityProvided;

/// <summary>
/// You need to create the stored procedure Update_ItemLock, which can be found under Database->CommunityProvided->F3rreNotificationService
/// </summary>
public class FerreNotificationService : INotificationService
{
    private readonly AccountContext _accountContext;
    private readonly ILogger<FerreNotificationService> _logger;

    public FerreNotificationService(AccountContext accountContext, ILogger<FerreNotificationService> logger)
    {
        _accountContext = accountContext;
        _logger = logger;
    }
    
    private int UpdateLockPw(string JID, string Email,string LockPW) =>
        _accountContext.Database
            .SqlQuery<int?>($"EXEC Update_ItemLock @jid = {JID}, @email = {Email}, @lockPw = {LockPW}")
            .AsEnumerable().FirstOrDefault() ?? -1;

    public Task<bool> SendSecondPassword(SendCodeRequest request)
    {
        _logger.LogWarning("Sending second password is not implemented! Failed for [{RequestEmail}]", request.email);
        return Task.FromResult(false);
    }

    public Task<bool> SendItemLockCode(SendCodeRequest request)
    {
        if (!request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return Task.FromResult(false);
        }

        if(UpdateLockPw(request.jid.ToString(), request.email,request.code) >= 0) 
            return Task.FromResult(true);
        
        _logger.LogError("Sending Item Lock Key has failed for [{RequestEmail}]", request.email);
        return Task.FromResult(false);
    }
    
    
}