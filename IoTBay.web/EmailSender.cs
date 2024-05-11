using System.Net;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace IoTBay.web;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("dicky.evaldo11@gmail.com"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        email.Body = builder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        smtp.Connect("smtp.gmail.com", 465, true); // Replace 'smtp.example.com' with your SMTP server details
        smtp.Authenticate("dicky.evaldo11@gmail.com", "piqlyqqixkevfoyv"); // Use your SMTP credentials
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}