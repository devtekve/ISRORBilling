using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISRORBilling.Database;

[Table("TB_User")]
public class User
{
    [Key] 
    public int JID {get; set;}
    public int PortalJID {get; set;}
    public string StrUserID { get; set; } = "";
    public int ServiceCompany {get; set;}
    public string password { get; set; } = "";
    public int Active {get; set;}
    public string UserIP { get; set; } = "";
    public string CountryCode { get; set; } = "";
    public DateTime VisitDate {get; set;}
    public DateTime RegDate {get; set;}
    public byte sec_primary {get; set;}
    public byte sec_content {get; set;}
    public byte sec_grade {get; set;}
    public int AccPlayTime {get; set;}
    public int LatestUpdateTime_ToPlayTime {get; set;}

}