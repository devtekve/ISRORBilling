using ISRORBilling.Database;
using ISRORBilling.Models;
using Microsoft.EntityFrameworkCore;

namespace ISRORBilling.Services;

/// <summary>
/// For this auth type to work you must have the account on MU_User, you must have it as well on dbo.AUH_AgreedService, and it also must exist on TB_Users (becuse the procedure _CertifyTB_User_Integration relies on it, tho you can modify it)
/// This would support email authentication, tho maybe some tweaks need to happen on the stored procedure to handle it properly. The PW for email is stored in SHA256
/// </summary>
public class FullAuthService : IAuthService
{
	private readonly JoymaxPortalContext _joymaxPortalContext;

	public FullAuthService(JoymaxPortalContext joymaxPortalContext) => _joymaxPortalContext = joymaxPortalContext;

	public AUserLoginResponse Login(string userId, string userPw, string channel)
       {
	       var loginResponse = _joymaxPortalContext.AUserLoginResponses.FromSqlInterpolated(
            $@"
            DECLARE @return_value int
            , @UserID varchar(30)
            , @UserPwd char(32)
            , @ServiceCode smallint
            , @UserIP binary(4)
            , @Channel char(1)
            , @JID int
            , @CurrentDate datetime
            , @ARCode smallint
            , @EmailAddr varchar(128)
            , @EmailCertificationStatus char(1)
            , @EmailUniqueStatus char(1)
            , @NickName varchar(30)
            , @VipLevel int
            , @VipExpireTime varchar(20)
            , @VipUserType int;

            EXEC @return_value = [dbo].[A_UserLogin] @UserID = {userId},
                    @UserPwd = {userPw},
                    @ServiceCode = 1,
                    @UserIP = 0,
                    @Channel = {channel},
                    @JID = @JID OUTPUT,
                    @CurrentDate = @CurrentDate OUTPUT,
                    @ARCode = @ARCode OUTPUT,
                    @EmailAddr = @EmailAddr OUTPUT,
                    @EmailCertificationStatus = @EmailCertificationStatus OUTPUT,
                    @EmailUniqueStatus = @EmailUniqueStatus OUTPUT,
                    @NickName = @NickName OUTPUT,
                    @VipLevel = @VipLevel OUTPUT,
                    @VipExpireTime = @VipExpireTime OUTPUT,
                    @VipUserType = @VipUserType OUTPUT;

			SELECT	'ReturnValue' = @return_value,
					@JID as N'JID',
					@CurrentDate as N'CurrentDate',
					@ARCode as N'ARCode',
					@EmailAddr as N'EmailAddr',
					@EmailCertificationStatus as N'EmailCertificationStatus',
					@EmailUniqueStatus as N'EmailUniqueStatus',
					@NickName as N'NickName',
					@VipLevel as N'VipLevel',
					@VipExpireTime as N'VipExpireTime',
					@VipUserType as N'VipUserType';");
	       
		return loginResponse.AsEnumerable().FirstOrDefault() ?? new AUserLoginResponse()
			{ ReturnValue = LoginResponseCodeEnum.NotFoundUid };
       }
}