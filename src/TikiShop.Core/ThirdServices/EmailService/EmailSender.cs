﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using TikiShop.Model.Configurations;

namespace TikiShop.Core.ThirdServices.EmailService;

public class EmailSender : IEmailSender<User>
{
    private readonly ILogger<EmailSender> _logger;
    private readonly SmtpConfig _smtpConfig;

    public EmailSender(ILogger<EmailSender> logger, IOptions<SmtpConfig> smtpOptions)
    {
        _logger = logger;
        _smtpConfig = smtpOptions.Value;
    }

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        var message = CreateEmailMessage(user, email, "Confirm your email",
            $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        await SendAsync(message);
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        var message = CreateEmailMessage(user, email, "Reset your password",
            $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
        await SendAsync(message);
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        var message = CreateEmailMessage(user, email, "Reset your password",
            $"Please reset your password using the following code: {resetCode}");
        await SendAsync(message);
    }

    private MimeMessage CreateEmailMessage(User user, string email, string subject, string bodyText)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtpConfig.UserName, _smtpConfig.Host));
        message.To.Add(new MailboxAddress(user.UserName, email));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = bodyText };

        _logger.LogInformation($"Email message created: {subject} for {user.UserName} at {email}");
        return message;
    }

    private async Task SendAsync(MimeMessage message)
    {
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port, true);
            await client.AuthenticateAsync(_smtpConfig.UserName, _smtpConfig.Password);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.SendAsync(message);
            _logger.LogInformation($"Email sent: {message.Subject} to {message.To}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the email.");
        }
        finally
        {
            await client.DisconnectAsync(true);
            _logger.LogInformation("Disconnected from SMTP server.");
        }
    }
}