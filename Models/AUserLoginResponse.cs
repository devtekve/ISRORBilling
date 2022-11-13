using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace ISRORBilling.Models;

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
    public string? VipLevel { get; set; }
    public DateTime? VipExpireTime { get; set; }
    public VipUserType? VipUserType { get; set; }

    public override string ToString()
    {
        return $"{(int)ReturnValue}|" + 
               $"{JID}|" +
               $"{(CurrentDate ?? DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)}|" +
               $"{EmailCertificationStatus}|" + // Not confirmed
               $"{EmailUniqueStatus}|" + // Not 100% confirmed, it could be either EmailUniqueStatus or EmailCertificationStatus, but's one of those.
               $"{EmailAddr ?? "NULL"}|" +
               $"{VipLevel}|" +
               $"{(VipExpireTime ?? DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)}|" +  
               $"{VipUserType}|" + 
               $"AFAILUREDESCRIPTION?|" + //This is named "desc" on the error message on the gateway when there's a failure during login.
               $"URL-RELATED?|" ; //This is named "url" on the error message on the gateway when there's a failure during login.
    }
}