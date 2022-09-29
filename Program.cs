using ISRORBilling.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();

builder.Services.AddDbContext<AccountContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("DbConfig").Value);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

var app = builder.Build();

app.MapGet("/Property/Silkroad-r/checkuser.aspx",
    ([FromQuery] string values, [FromServices] AccountContext accountContext) =>
    {
        var allValues = values.Split('|');
        if (allValues[0] == "2")
            return "2";

        var user = accountContext.Users.FirstOrDefault(x => x.StrUserID == allValues[1] && x.password == allValues[2]);
        if (user != null)
            return $"0|{user.PortalJID}";

        return "1";
    });

app.Run();