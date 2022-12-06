using System.Globalization;
using ISRORBilling.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ISRORBilling.Models.Authentication;

[Keyless]
public class AUserLoginResponse
{
    public LoginResponseCodeEnum ReturnValue { get; set; }
    public int? JID { get; set; }
    public DateTime? CurrentDate { get; set; }
    public short? ARCode { get; set; }
    public string? EmailAddr { get; set; }
    public string? EmailCertificationStatus { get; set; }
    public string? EmailUniqueStatus { get; set; }
    public string? NickName { get; set; }
    public int? VipLevel { get; set; }
    public DateTime? VipExpireTime { get; set; }
    public VipUserType? VipUserType { get; set; }
    public string? CountryCode { get; set; }

    public override string ToString()
    {
        return $"{(int)ReturnValue}|" + 
               $"{JID}|" +
               $"{(CurrentDate ?? DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)}|" +
               $"{CountryCode ?? "ZZ"}|" + // THIS IS COUNTRYCODE USED IN [SILKROAD_R_SHARD].[dbo].[_ActiveUser]
               $"{EmailCertificationStatus}|" + // Not 100% confirmed, it could be either EmailUniqueStatus or EmailCertificationStatus, but's one of those.
               $"{EmailAddr ?? "NULL"}|" +
               $"{VipLevel ?? 0}|" +
               $"{(VipExpireTime ?? DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)}|" +
               $"{(int)(VipUserType ?? Enums.VipUserType.Free)}|" + 
               "UIIT_MSG_USERID_TOO_MANY_LOGIN_TRY_BLOCK|" + //testing this part, This is named "desc" on the error message on the gateway when there's a failure during login.
               "URL-RELATED?" ; //This is named "url" on the error message on the gateway when there's a failure during login.
    }
}