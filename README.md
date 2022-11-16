# ISRORBilling
As this is an educational project, the idea is to use it to learn new ways of working and help each other improve.


This tool handles the login from users into ISROR files; It has been designed extensible, so you can create your own login flow if you want to.

## New Update 16/11/2022 (Nemo07)

- Added missing info to SQL Script .

1-Excute this [Update_ItemLock.sql Script](/Database/CommunityProvided/F3rreNotificationService/Update_ItemLock.sql)

2-Excute this **``` ALTER TABLE [SILKROAD_R_ACCOUNT].[dbo].[TB_User] ADD [ItemLockPW] VARCHAR(max); ```**

3-Choose **Type : Ferre** from "*appsettings.json*" 👈👀

![App Screenshot](https://i.imgur.com/Ph3nPcb.png)

- Enabled TCP system , Thanks to Ferre for sure ..

![App Screenshot](https://i.imgur.com/iOMPFBL.png)

### Here is how to make TCP work.

1-Open General Server.cfg.

2-Go to Gateway Section to "**TcpPingServerCount**".

3-Change "TcpPingServerCount" from 3 to 4.

4-Add 

```	// TcpPing01_Nation : Europe
	TcpPing03_Nation		4			// Second (Zero Base) Bing Server Country Code
	TcpPing03_URL			"YourServerIP"	// Second Ping Server URL
	TcpPing03_Port			12989			// Second Ping Server Port
```

![App Screenshot](https://i.imgur.com/T5laM3T.png)


## Appsettings layout
Appsettings is where you can configure the tool's behavior.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://0.0.0.0:18080" // 👈 Listening address. Note the port 18080 to avoid collisions.
      }
    }
  },
  "AuthService": "Simple", // 👈 Supported types: Simple, Full, Bypass, Nemo.
  "DbConfig": {
    "AccountDB": "Data Source=.\\;TrustServerCertificate=True;Initial Catalog=SILKROAD_R_ACCOUNT;User ID=sa;Password=1;",
    "JoymaxPortalDB": "Data Source=.\\;TrustServerCertificate=True;Initial Catalog=GB_JoymaxPortal;User ID=sa;Password=1;"
  },
  "NotificationService": {
    "Type": "Email" // 👈 Only supported email for now. In the future others can add more types, same as auth type. //Email (SMTP) //Ferre (bypassed the code in TB_User table)
  },
  "EmailService": {
    "From": "yourEmail",
    "FromFriendlyName": "YourServerName??",
    "SmtpServer": "smtp.gmail.com",
    "Port": 465,
    "Username": "FOLLOW https://code-maze.com/aspnetcore-send-email/",
    "Password": "FOLLOW https://code-maze.com/aspnetcore-send-email/",
    "SkipTokenValidation": false
  },
  "SaltKey": "eset5ag.nsy-g6ky5.mp",  // 👈 Used to validate payloads in some of the auth services. it must match the GatewayServer hardcoded value!
  "AllowedHosts": "*" // 👈 learn more: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/host-filtering?view=aspnetcore-6.0
}
```
## Authentication Service Types
There's 3 auth service types supported by this tool to provide different levels of customization. 

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