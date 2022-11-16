using ISRORBilling.Database;
using ISRORBilling.Database.CommunityProvided.Nemo07;
using ISRORBilling.Models.Authentication;
using ISRORBilling.Models.Notification;
using ISRORBilling.Models.Options;
using ISRORBilling.Services.Authentication;
using ISRORBilling.Services.Authentication.CommunityProvided.Nemo07;
using ISRORBilling.Services.Notification;
using ISRORBilling.Services.Notification.CommunityProvided;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using ISRORBilling.Tcp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddDbContext<AccountContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("DbConfig")["AccountDB"]);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddDbContext<JoymaxPortalContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("DbConfig")["JoymaxPortalDB"]);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

Enum.TryParse(builder.Configuration.GetSection("NotificationService:Type")?.Value, true, out NotificationServiceType notificationServiceType);
switch (notificationServiceType)
{
    case NotificationServiceType.Email:
        builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailService"));
        builder.Services.AddSingleton<INotificationService, EmailNotificationService>();
        break;
    
    case NotificationServiceType.Ferre:
        builder.Services.AddSingleton<INotificationService, FerreNotificationService>();
        break;
    
    case NotificationServiceType.None:
    default:
        builder.Services.AddSingleton<INotificationService, NoneNotificationService>();
        break;
}

Enum.TryParse(builder.Configuration.GetSection("AuthService")?.Value, true, out SupportedLoginServicesEnum loginService);
switch (loginService)
{
    case SupportedLoginServicesEnum.Full:
        NationPing stsFull = new NationPing();
        builder.Services.AddScoped<IAuthService, FullAuthService>();
        break;
    case SupportedLoginServicesEnum.Bypass:
        builder.Services.AddScoped<IAuthService, BypassAuthService>();
        break;
    case SupportedLoginServicesEnum.Nemo:
    NationPing stsNemo = new NationPing();
        builder.Services.AddDbContext<NemoAccountContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetSection("DbConfig")["AccountDB"]);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        builder.Services.AddScoped<IAuthService, NemoAuthService>();
        break;

    case SupportedLoginServicesEnum.Simple:
    default:
        builder.Services.AddScoped<IAuthService, SimpleAuthService>();
        break;
}

var saltKey = builder.Configuration.GetSection("SaltKey").Value ?? string.Empty;
var app = builder.Build();


app.MapGet("/Property/Silkroad-r/checkuser.aspx",
    ([FromQuery] string values, [FromServices] ILogger<Program> logger, [FromServices] IAuthService authService) =>
    {
        if(saltKey.IsNullOrEmpty())
            logger.LogWarning("THERE'S NO SALT KEY CONFIGURED IN APPSETTINGS; WE CAN'T VALIDATE IF REQUEST WAS TAMPERED!");
        
        logger.LogDebug("Received in params: {Values}", values);
        
        var request = new CheckUserRequest(values, saltKey);
        return authService.Login(request).ToString();
    });

app.MapGet("/cgi/EmailPassword.asp",
    async ([FromQuery] string values, [FromServices] ILogger<Program> logger, [FromServices] AccountContext accountContext,
        [FromServices] INotificationService notificationService) =>
    {
        logger.LogDebug("Received in params: {Values}", values);
        var request = new SendCodeRequest(values, saltKey);

        if (await notificationService.SendSecondPassword(request))
            return 0;

        return -1;
    });

app.MapGet("/cgi/Email_Certification.asp",
    async ([FromQuery] string values, [FromServices] ILogger<Program> logger, [FromServices] AccountContext accountContext,
        [FromServices] INotificationService notificationService) =>
    {
        if (saltKey.IsNullOrEmpty())
            logger.LogWarning("THERE'S NO SALT KEY CONFIGURED IN APPSETTINGS; WE CAN'T VALIDATE IF REQUEST WAS TAMPERED!");

        logger.LogDebug("Received in params: {Values}", values);

        var request = new SendCodeRequest(values, saltKey);

        if (await notificationService.SendItemLockCode(request))
            return 0;

        return -1;
    });
app.Run();