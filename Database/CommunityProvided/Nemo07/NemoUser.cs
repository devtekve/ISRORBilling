using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISRORBilling.Database;

/// <summary>
/// In order to use this, you must modify your existing TB_User table and add the fields below, more info on the readme!.
/// </summary>
public class NemoUser : User
{
    public string? Email { get; set; }
    public string? EmailCertificationStatus { get; set; }
    public string? EmailUniqueStatus { get; set; }
    public int VIPLv { get; set; }
    public DateTime VipExpireTime { get; set; }
    public int VipUserType { get; set; }
}