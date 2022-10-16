using ISRORBilling.Database;
using ISRORBilling.Models;
using ISRORBilling.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        builder.Services.AddScoped<IAuthService, SimpleAuthService>();
        break;
    case SupportedLoginServicesEnum.Bypass:
        builder.Services.AddScoped<IAuthService, BypassAuthService>();
        break;
    
    case SupportedLoginServicesEnum.Simple:
    default:
        builder.Services.AddScoped<IAuthService, SimpleAuthService>();
        break;
}

builder.Services.AddSingleton<IEmailService, EmailService>();
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
        [FromServices] IEmailService emailService) =>
    {
        logger.LogDebug("Received in params: {Values}", values);
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