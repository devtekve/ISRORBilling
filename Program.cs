using ISRORBilling.Database;
using ISRORBilling.Models;
using ISRORBilling.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

Enum.TryParse(builder.Configuration.GetSection("AuthService")?.Value, true, out SupportedLoginServicesEnum loginService);
switch (loginService)
{
    case SupportedLoginServicesEnum.Full:
        builder.Services.AddSingleton<IAuthService, SimpleAuthService>();
        break;
    case SupportedLoginServicesEnum.Bypass:
        builder.Services.AddSingleton<IAuthService, BypassAuthService>();
        break;
    
    case SupportedLoginServicesEnum.Simple:
    default:
        builder.Services.AddSingleton<IAuthService, SimpleAuthService>();
        break;
}

builder.Services.AddSingleton<IEmailService, EmailService>();
var app = builder.Build();


app.MapGet("/Property/Silkroad-r/checkuser.aspx",
    ([FromQuery] string values, [FromServices] IAuthService authService) =>
    {
        var allValues = values.Split('|');
        var channel = allValues[0];
        var userId = allValues[1];
        var userPw = allValues[2];

        return authService.Login(userId, userPw, channel).ToString();
    });

app.MapGet("/cgi/EmailPassword.asp",
    async ([FromQuery] string values, [FromServices] AccountContext accountContext,
        [FromServices] IEmailService emailService) =>
    {
        var allValues = values.Split('|');
        var channel = allValues[0];
        var newCode = allValues[1];
        var email = allValues[2];
        var token = allValues[3];

        if (await emailService.SendSecondPasswordByEmail(newCode, email))
            return 0;

        return -1;
    });
app.Run();