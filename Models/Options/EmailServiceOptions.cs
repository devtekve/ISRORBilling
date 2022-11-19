namespace ISRORBilling.Models.Options;

/// <summary>
/// Thanks https://code-maze.com/aspnetcore-send-email/
/// </summary>
public class EmailOptions
{
    public string From { get; } = "unknown@unknown.com";
    public string FromFriendlyName { get; } = "UnknownFriendlyName";
    public string SmtpServer { get; } = "";
    public bool UseSSL { get; } = true;
    public int Port { get; }
    public string UserName { get; } = "";
    public string Password { get; } = "";
    public bool SkipTokenValidation { get; }
}