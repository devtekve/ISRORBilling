namespace ISRORBilling.Models;

public enum LoginResponseCodeEnum : int
{
    Success = 0,
    WrongPassword = 1,
    ServerMaintenance = 2,
    C7 = 3,
    NeedVerification = 4,
    Error = -65543,
    NoRowsAffectedMaybe = -65544,
    Emergency = -65553,
    WrongPassword = -131077,
    BlockedJid = -131078,
    NotFoundUid = -131079, //Check if user is on dbo.MU_User
    BlockedIp = -131080,
    NotSubscribedToService = -131081, //Check if JID is in dbo.AUH_AgreedService
    BlockedCountry = -131084,
    EmailAuthNotUsed = -262145
}
