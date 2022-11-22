using ISRORBilling.Models.Authentication;
using ISRORBilling.Models.Notification;
using ISRORBilling.Models.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ISRORBilling.Services.Notification;

/// <summary>
/// If using GMAIL, follow https://code-maze.com/aspnetcore-send-email/ , under "How to Enable Less Secure Apps with Gmail"
/// </summary>
public class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly EmailOptions _emailOptions;
    private MimeMessage _mimeMessage = null!;

    public EmailNotificationService(ILogger<EmailNotificationService> logger, IOptions<EmailOptions> emailOptions)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
    }

    /// <summary>
    /// Sends the mail to the user
    /// </summary>
    /// <param name="mailMessage"></param>
    /// <returns>True if the sending worked; False if the sending failed.</returns>
    private bool SendEmail(MimeMessage mailMessage)
    {
        bool result;

        using var client = new SmtpClient();
        try
        {
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.CheckCertificateRevocation = _emailOptions.UseCCR;
            client.Connect(_emailOptions.SmtpServer, _emailOptions.Port, _emailOptions.UseSSL);
            client.Authenticate(_emailOptions.UserName, _emailOptions.Password);
            client.Send(mailMessage);
            result = true;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to send the email! {E}\n------\nMimeMessage: \n{Message}\n----", e.Message, mailMessage);
            result = false;
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }

        return result;
    }

    public async Task<bool> SendSecondPassword(SendCodeRequest request)
    {
        if (!_emailOptions.SkipTokenValidation && !request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return await Task.FromResult(false);
        }

        if (!_emailOptions.PasswordTemplate.IsNullOrEmpty())
        {
            if (File.Exists(_emailOptions.PasswordTemplate))
            {
                _mimeMessage = new EmailMessage(request.email, $"[{_emailOptions.FromFriendlyName}] New secondary password!", UpdateTemplate(_emailOptions.PasswordTemplate, _emailOptions.FromFriendlyName, request.code).Result)
                    .ToMimeMessageBase64(_emailOptions.FromFriendlyName, _emailOptions.From);
            }
            else
            {
                _mimeMessage = new EmailMessage(request.email, $"[{_emailOptions.FromFriendlyName}] New secondary password!", $"Your new secondary password for server [{_emailOptions.FromFriendlyName}] is [{request.code}]")
                    .ToMimeMessage(_emailOptions.FromFriendlyName, _emailOptions.From);
            }
        }
        else
        {
            _mimeMessage = new EmailMessage(request.email, $"[{_emailOptions.FromFriendlyName}] New secondary password!", $"Your new secondary password for server [{_emailOptions.FromFriendlyName}] is [{request.code}]")
                .ToMimeMessage(_emailOptions.FromFriendlyName, _emailOptions.From);
        }

        if (SendEmail(_mimeMessage))
            return await Task.FromResult(true);

        _logger.LogError("Sending second password by email has Failed for [{StrEmail}]", request.email);
        return await Task.FromResult(false);
    }

    public async Task<bool> SendItemLockCode(SendCodeRequest request)
    {
        if (!_emailOptions.SkipTokenValidation && !request.Validate())
        {
            _logger.LogCritical("Couldn't validate if request was legitimate. Ensure the SaltKey matches the one in GatewayServer. [Error Code: {ErrorCode}]\nDetails:{Request}", (int)LoginResponseCodeEnum.Emergency, request);
            return await Task.FromResult(false);
        }

        if (!_emailOptions.PasscodeTemplate.IsNullOrEmpty())
        {
            if (File.Exists(_emailOptions.PasscodeTemplate))
            {
                _mimeMessage = new EmailMessage(request.email, $"[{_emailOptions.FromFriendlyName}] Item Lock Code", UpdateTemplate(_emailOptions.PasscodeTemplate, _emailOptions.FromFriendlyName, request.code).Result)
                    .ToMimeMessageBase64(_emailOptions.FromFriendlyName, _emailOptions.From);
            }
            else
            {
                _mimeMessage = new EmailMessage(request.email, $"[{_emailOptions.FromFriendlyName}] Item lock code!", $"Your item lock code for server [{_emailOptions.FromFriendlyName}] is [{request.code}]")
                    .ToMimeMessage(_emailOptions.FromFriendlyName, _emailOptions.From);
            }
        }
        else
        {
            _mimeMessage = new EmailMessage(request.email, $"[{_emailOptions.FromFriendlyName}] Item lock code!", $"Your item lock code for server [{_emailOptions.FromFriendlyName}] is [{request.code}]")
                .ToMimeMessage(_emailOptions.FromFriendlyName, _emailOptions.From);
        }

        if (SendEmail(_mimeMessage))
            return await Task.FromResult(true);

        _logger.LogError("Sending Item Lock Key by email has failed for [{RequestEmail}]", request.email);
        return await Task.FromResult(false);
    }

    private static async Task<string> UpdateTemplate(string file, string name, string code)
    {
        return await Task.Run(() => File.ReadAllText(file, Encoding.UTF8)
            .Replace("%DateTime%", DateTime.Now.ToString("U"))
            .Replace("%ServerName%", name)
            .Replace("%PasscodeText%", code));
    }
}