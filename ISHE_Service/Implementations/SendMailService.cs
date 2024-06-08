using ISHE_Service.Interfaces;
using ISHE_Utility.Settings;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ISHE_Service.Implementations
{
    public class SendMailService : ISendMailService
    {
        private readonly string _nameApp;
        private readonly string _emailAddress;
        private readonly bool _useSSL;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _useStartTls;
        private readonly string _username;
        private readonly string _password;
        private readonly static CancellationToken ct = new CancellationToken();

        public SendMailService(IOptions<AppSetting> appSettings)
        {
            _nameApp = appSettings.Value.NameApp;
            _emailAddress = appSettings.Value.EMailAddress;
            _useSSL = appSettings.Value.UseSSL;
            _host = appSettings.Value.Host;
            _port = appSettings.Value.Port;
            _useStartTls = appSettings.Value.UseStartTls;
            _username = appSettings.Value.UserName;
            _password = appSettings.Value.Password;
        }

        public async Task SendEmail(string userEmail, string title, string message)
        {

            try
            {
                var mail = new MimeMessage();

                // Sender
                mail.From.Add(new MailboxAddress(_nameApp, _emailAddress));
                mail.Sender = new MailboxAddress(_nameApp, _emailAddress);

                // Receiver
                mail.To.Add(MailboxAddress.Parse(userEmail));

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = $"{title}";
                body.HtmlBody = $"<p>{message}</p>";
                mail.Body = body.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                if (_useSSL)
                {
                    await smtp.ConnectAsync(_host, _port, SecureSocketOptions.SslOnConnect, ct);
                }
                else if (_useStartTls)
                {
                    await smtp.ConnectAsync(_host, _port, SecureSocketOptions.StartTls, ct);
                }
                await smtp.AuthenticateAsync(_username, _password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
