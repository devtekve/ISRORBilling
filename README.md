# ISRORBilling
- [ISRORBilling](#isrorbilling)
    * [Appsettings layout](#appsettings-layout)
    * [About services](#about-services)
        + [Authentication Service](#authentication-service)
        + [Notification Service](#notification-service)
        + [Nation Ping Service](#nation-ping-service)

<small><i><a href='http://ecotrust-canada.github.io/markdown-toc/'>Table of contents generated with markdown-toc</a></i></small>


As this is an educational project, the idea is to use it to learn new ways of working and help each other improve.


This tool handles the login from users into ISROR files; It has been designed extensible, so you can create your own login flow if you want to.

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
    "Type": "Email" // 👈 Email (recommended) or Ferre
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
  "NationPingService": {
    "ListenAddress": "0.0.0.0",
    "ListenPort": 12989
  },
  "SaltKey": "eset5ag.nsy-g6ky5.mp",  // 👈 Used to validate payloads in some of the auth services. it must match the GatewayServer hardcoded value!
  "AllowedHosts": "*" // 👈 learn more: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/host-filtering?view=aspnetcore-6.0
}
```

## About services
We try to follow "micro-service" architechture approach, this is done to allow multiple different implementations of a given service, so that the community can easily customize the behavior of their application.

We currently have 3 types of services


### Authentication Service
This is the service used to authenticate a given user that's trying to login on the game. It's called by the gatewayserver via HTTP requests.

#### [Read more about Authentication Services here](/Services/Authentication/README.md)

### Notification Service
The notification service is used to send notifications such as secondary password reset or item lock to the user. This can be done via multiple implementations of the service.

#### [Read more about Notification Services here](/Services/Notification/README.md)

### Nation Ping Service
This is a service used to show the ping of the given server from the ingame login screen.
![App Screenshot](https://i.imgur.com/iOMPFBL.png)

#### [Read more about Ping Services here](/Services/Ping/README.md)