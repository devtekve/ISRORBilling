namespace ISRORBilling;

public class GenericHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GenericHandlerMiddleware> _logger;

    public GenericHandlerMiddleware(RequestDelegate next, ILogger<GenericHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);
        var request = context.Request;
        var response = context.Response;
        if (response.StatusCode == StatusCodes.Status404NotFound)
            _logger.LogWarning($"Unhandled [{request.Method}] request received to: {request.Path}{request.QueryString}");
    }
}