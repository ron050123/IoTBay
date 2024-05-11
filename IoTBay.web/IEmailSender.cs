namespace IoTBay.web;

using MailKit.Net.Smtp;
using MimeKit;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}
