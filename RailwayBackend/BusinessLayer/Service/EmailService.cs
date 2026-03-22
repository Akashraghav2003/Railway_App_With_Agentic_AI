using System;
using System.Net;
using System.Net.Mail;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using ModelLayer.Models;

namespace BusinessLayer.Service;

public class EmailService : IEmailService
{
    
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendEmail(EmailModel emailModel)
    {
        try
        {
            var host = _config["SMTP:Host"] ?? throw new InvalidOperationException("SMTP Host configuration is missing");
            var portString = _config["SMTP:Port"] ?? throw new InvalidOperationException("SMTP Port configuration is missing");
            var username = _config["SMTP:Username"] ?? throw new InvalidOperationException("SMTP Username configuration is missing");
            var password = _config["SMTP:Password"] ?? throw new InvalidOperationException("SMTP Password configuration is missing");
            
            using var client = new SmtpClient(host, int.Parse(portString))
            {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
            From = new MailAddress(username),
            Subject = emailModel.Subject,
            Body = emailModel.Body,
            IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(emailModel.To));

            client.Send(mailMessage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending email: {ex.Message}", ex);
        }

    }
}
