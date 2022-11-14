using ISRORBilling.Models.Authentication;
using ISRORBilling.Models.Notification;
using ISRORBilling.Models.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ISRORBilling.Services.Notification;

/// <summary>
/// If using GMAIL, follow https://code-maze.com/aspnetcore-send-email/ , under "How to Enable Less Secure Apps with Gmail"
/// </summary>
public class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly EmailOptions _emailOptions;

    public EmailNotificationService(ILogger<EmailNotificationService> logger, IOptions<EmailOptions> emailOptions)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
    }
    
    private bool SendEmail(MimeMessage mailMessage)
    {
        bool result;
        using var client = new SmtpClient();
        try
        {
            client.Connect(_emailOptions.SmtpServer, _emailOptions.Port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(_emailOptions.UserName, _emailOptions.Password);
            client.Send(mailMessage);
            result = true;
        }
        catch(Exception e)
        {
            _logger.LogError("Failed to send the email! {E}\n------\nMimeMessage: \n{Message}\n----", e, mailMessage);
            result = false;
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }

        return result;
    }

    public Task<bool> SendSecondPassword(SendCodeRequest request)
    {
        if (!_emailOptions.SkipTokenValidation && !request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return Task.FromResult(false);
        }
        
        var mimeMessage = new EmailMessage(request.email, $"[{_emailOptions.FromFriendlyName}] New secondary password!", $"Your new secondary password for server [{_emailOptions.FromFriendlyName}] is [{request.code}]")
            .ToMimeMessage(_emailOptions.From, _emailOptions.From);
        
        if(SendEmail(mimeMessage)) 
            return Task.FromResult(true);
        
        _logger.LogError("Sending second password by email has Failed for [{StrEmail}]", request.email);
        return Task.FromResult(false);
    }

    public Task<bool> SendItemLockCode(SendCodeRequest request)
    {
        if (!_emailOptions.SkipTokenValidation && !request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return Task.FromResult(false);
        }

        var mimeMessage = new EmailMessage(request.email, $"[{_emailOptions.FromFriendlyName}] Item lock code!", $"Your item lock code for server [{_emailOptions.FromFriendlyName}] is [{request.code}]")
            .ToMimeMessage(_emailOptions.FromFriendlyName, _emailOptions.From);
        
        if(SendEmail(mimeMessage)) 
            return Task.FromResult(true);
        
        _logger.LogError("Sending Item Lock Key by email has failed for [{RequestEmail}]", request.email);
        return Task.FromResult(false);
    }
    
    
}