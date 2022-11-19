# Authentication Service
This is the service used to authenticate a given user that's trying to login on the game. It's called by the gatewayserver via HTTP requests.

## Authentication Service Types
There's 4 auth service types supported by this tool to provide different levels of customization.

### Simple
When the property `AuthService` in `appsettings.json` is set to `Simple`, the tool will go to the usual vsro place.
That is, `TB_Users` to do the login validation

```json
{
  "AuthService": "Simple", // 👈👀
}
```

### Full
When the property `AuthService` in `appsettings.json` is set to `Full`, the login flow uses the original login flow stored procedure on this files;
Consuming `[GB_JoymaxPortal].[dbo].[A_UserLogin]` stored procedure.

For this auth type to work you must:
* Have the account on `[GB_JoymaxPortal].[dbo].[MU_User]`
    * The PW for email is stored in `SHA256`; **NOT** md5 as vsro / simple auth.
* Have it as well on `[GB_JoymaxPortal].[dbo].[dbo.AUH_AgreedService]`
* It also must exist on TB_Users (becuse the procedure `_CertifyTB_User_Integration` relies on it, tho you can modify it)

This would support email authentication, however, some tweaks are needed to make it as the stored procedure treats the UserId as int for JC.

```json
{
  "AuthService": "Full", // 👈👀
}
```

### Bypass
You like living on the edge, and security is just an inconvinient myth.
When the property `AuthService` in `appsettings.json` is set to `Bypass`, the login flow uses `[TB_User]` and just finds the JID for the UserId you've provided during the login;
It completely **bypasses the password**

```json
{
  "AuthService": "Bypass", // 👈👀
}
```

### Nemo

It's a feature that make you use **VIP system** with simple authorizer for people who can't deal with tables in **GB_JoymaxPortal** , you will only use the **[TB_User]** table in **Silkroad_R_Account** Database.

`appsettings.json`
```json
{
  ...
  "AuthService": "Nemo", // 👈 Changed here to Nemo
  "DbConfig": {...},
  ...
}
```

You can see [more details here](/Services/CommunityProvided/Nemo07#made-by-nemo07)


## Make your own login flow
Thanks to dependency injection, **you don't need to modify the endpoints themselves**. If you have a newer / better / funnier / cooler authentication flow, you can create it and have minimal changes on the app itself. You just need to make sure you implement the interface `IAuthService`.
### Example of very simple auth service that doesn't do DB
`Services/NoDbAuthService.cs`
```csharp
namespace ISRORBilling.Services;

public class NoDbAuthService : IAuthService // 👈 Note we implement IAuthService
{
    public AUserLoginResponse Login(string userId, string userPw, string channel)
    {
        return new AUserLoginResponse
        {
            ReturnValue = LoginResponseCodeEnum.Success,
            JID = 1 // 👈 Whatever JID you want to hardcode
        };
    }
}
```

`Models/SupportedLoginServicesEnum.cs`
```csharp
namespace ISRORBilling.Models;

public enum SupportedLoginServicesEnum: byte
{
    Simple,
    Full,
    Bypass,
    NoDb // 👈 New service name, to be used on the appsettings.json later
}
```
`appsettings.json`
```json
{
  "AuthService": "NoDb", // 👈👀
}
```

`Program.cs`
```csharp
//... skipped for brevety ...
Enum.TryParse(builder.Configuration.GetSection("AuthService")?.Value, true, out SupportedLoginServicesEnum loginService);
switch (loginService)
{
    case SupportedLoginServicesEnum.Full:
        builder.Services.AddSingleton<IAuthService, SimpleAuthService>();
        break;
    case SupportedLoginServicesEnum.Bypass:
        builder.Services.AddSingleton<IAuthService, BypassAuthService>();
        break;
    case SupportedLoginServicesEnum.NoDb: // 👈 Enum created earlier, matching the service name.
        builder.Services.AddSingleton<IAuthService, NoDbAuthService>(); // 👈 New implementation created earlier, note we use NoDbAuthService.
        break;
    default:
        builder.Services.AddSingleton<IAuthService, SimpleAuthService>();
        break;
}
//... skipped for brevety ...
```