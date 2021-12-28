using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

// Extends IEmailSender for Identity UI
public class SendMailService : IEmailSender
{
    private readonly MailSettings mailSettings;
    private readonly ILogger<SendMailService> logger;

    public SendMailService(IOptions<MailSettings> _mailSettings, ILogger<SendMailService> _logger)
    {
        // 9.3 Get _mailSettings in Options
        mailSettings = _mailSettings.Value;
        logger = _logger;
    }
    // 9.4 Implement SendEmailAsync for IEmailSender
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
        message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));

        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = htmlMessage;
        message.Body = builder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        try
        {

            await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
            await smtp.SendAsync(message);
        }
        catch (System.Exception e)
        {
            // Send mail failure, save mail into MailsSave
            Directory.CreateDirectory("MailsSave");
            var emailSaveFile = string.Format(@"MailsSave/{0}.eml", Guid.NewGuid());
            await message.WriteToAsync(emailSaveFile);

            logger.LogInformation("Send mail failure, Saved at " + emailSaveFile);
            logger.LogError(e.Message);
        }

        smtp.Disconnect(true);

        logger.LogInformation("Sent mail to " + email);
    }
}