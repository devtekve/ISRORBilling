using ISRORBilling.Models.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling;

public class GenericHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GenericHandlerMiddleware> _logger;
    private readonly string? _PortalCGIAgentHeader;
    private readonly string? _saltKey;

    public GenericHandlerMiddleware(RequestDelegate next, ILogger<GenericHandlerMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _PortalCGIAgentHeader = configuration.GetSection("PortalCGIAgentHeader").Value;
        _saltKey = configuration.GetSection("SaltKey").Value;

    }

    public async Task Invoke(HttpContext context)
    {
        var response = context.Response;
        var request = context.Request;

        var userAgentHeader = context.Request.Headers.UserAgent;
        if(_saltKey.IsNullOrEmpty())
            _logger.LogWarning("THERE'S NO SALT KEY CONFIGURED IN APPSETTINGS; WE CAN'T VALIDATE IF REQUEST WAS TAMPERED!");
        
        if (_PortalCGIAgentHeader.IsNullOrEmpty())
            _logger.LogWarning(
                "THERE'S NO PORTAL AGENT CONFIGURED IN APPSETTINGS; ANY BROWSER CAN BROWSE YOUR BILLING!");

        if (!_PortalCGIAgentHeader.IsNullOrEmpty() && userAgentHeader.Any(userAgent => userAgent == _PortalCGIAgentHeader))
        {
            _logger.LogCritical(
                "PORTAL AGENT DOES NOT MATCH; SOMEONE TRYING TO BROWSE YOUR BILLING URL FROM A NORMAL BROWSER");
            await response.WriteAsync(new AUserLoginResponse
                { ReturnValue = LoginResponseCodeEnum.BrowserAgentNotMatch }.ToString());
        }
        else
        {
            await _next(context);
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                _logger.LogWarning("Unhandled [{RequestMethod}] request received to: {RequestPath}{RequestQueryString}",
                    request.Method, request.Path, request.QueryString);
        }
    }
}