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
    public string? VipExpireTime { get; set; }
    public string? VipUserType { get; set; }

    public override string ToString()
    {
        return $"{(int)ReturnValue}|{JID}|unknown20byte|unknown2|{EmailUniqueStatus ?? "NULL"}|{EmailAddr ?? "NULL"}|unknown3|{(CurrentDate ?? DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)}|{NickName ?? "NULL"}";
    }
}