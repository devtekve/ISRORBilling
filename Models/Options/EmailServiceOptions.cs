namespace ISRORBilling.Models.Options;

/// <summary>
/// Thanks https://code-maze.com/aspnetcore-send-email/
/// </summary>
public class EmailOptions
{
    public string From { get; set; } = "unknown@unknown.com";
    public string FromFriendlyName { get; set; } = "UnknownFriendlyName";
    public string SmtpServer { get; set; } = "";
    public bool UseSSL { get; set; } = true;
    public bool UseCCR { get; set; } = true;
    public int Port { get; set; }
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public bool SkipTokenValidation { get; set; }
    public string PasswordTemplate { get; set; } = string.Empty;
    public string PasscodeTemplate { get; set; } = string.Empty;
}