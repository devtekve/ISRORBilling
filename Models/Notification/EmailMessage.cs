using MimeKit;

namespace ISRORBilling.Models.Notification;

public class EmailMessage
{
    public IEnumerable<MailboxAddress> To { get; }
    public string Subject { get; }
    public string Content { get; }
    
    public EmailMessage(IEnumerable<string> to, string subject, string content)
    {
        To = to.Select(x => new MailboxAddress(x, x));
        Subject = subject;
        Content = content;
    }
    
    public EmailMessage(string to, string subject, string content): this(new []{to}, subject, content)
    {
    }
    
    public MimeMessage ToMimeMessage(string senderName, string senderAddress)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(senderName,senderAddress));
        emailMessage.To.AddRange(To);
        emailMessage.Subject = Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = Content };
        return emailMessage;
    }

    public MimeMessage ToMimeMessageBase64(string senderName, string senderAddress)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(senderName,senderAddress));
        emailMessage.To.AddRange(To);
        emailMessage.Subject = Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = Content, ContentTransferEncoding = ContentEncoding.Base64};
        return emailMessage;
    }
}