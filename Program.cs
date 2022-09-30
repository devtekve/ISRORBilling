using ISRORBilling.Database;
using ISRORBilling.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();

builder.Services.AddDbContext<AccountContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("DbConfig").Value);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
builder.Services.AddSingleton<IEmailService, EmailService>();

var app = builder.Build();

var accContext = app.Services.GetService<AccountContext>();
accContext?.Database.ExecuteSql(
    $"IF COL_LENGTH('[TB_User]' , 'StrEmail') IS NULL ALTER TABLE [dbo].[TB_User] ADD StrEmail varchar(128)");

// To support the login with the second channel (email) the PW must be stored in SHA256; the companyId must be 2 and the JID must be in TB_User_Channel with the assigned channel = 2 and channelid = 2
// accContext?.Database.ExecuteSql($"IF COL_LENGTH('[TB_User]' , 'passwordSha256') IS NULL ALTER TABLE [dbo].[TB_User] ADD passwordSha256 varchar(128)");

app.MapGet("/Property/Silkroad-r/checkuser.aspx",
    ([FromQuery] string values, [FromServices] AccountContext accountContext) =>
    {
        var allValues = values.Split('|');
        var channel = allValues[0];
        var userId = allValues[1];
        var userPw = allValues[2];

        if (userId.IsNullOrEmpty()) return "1";
        var user = channel switch
        {
            "1" => accountContext.Users.FirstOrDefault(user => user.StrUserID == userId && user.password == userPw),
            // "2" => accountContext.Users.FirstOrDefault(user => user.StrEmail == userId && user.passwordSha256 == userPw),
            _ => null
        };

        if (user == null) return "1";

        return $"0|{user.PortalJID}|unknown1|unknown2|unknown3|{user.StrEmail}|unknown4|unknown5|unknown6";
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