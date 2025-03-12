using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace WPFTest.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly bool _isProduction;

        public EmailService(EmailSettings emailSettings, bool isProduction)
        {
            _emailSettings = emailSettings;
            _isProduction = isProduction;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (_isProduction)
            {
                // Utilisez les paramètres de configuration pour Gmail en production
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("no-reply@example.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await Task.Run(() => smtpClient.Send(mailMessage));
            }
            else
            {
                // Utilisez les paramètres de configuration pour Mailtrap en développement
                var smtpClient = new SmtpClient("smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("no-reply@example.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await Task.Run(() => smtpClient.Send(mailMessage));
            }
        }
    }
} 