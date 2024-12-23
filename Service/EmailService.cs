
using MailKit.Net.Smtp;
using MimeKit;
using News_Aggregator.DTO;

namespace News_Aggregator.Service
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(EmailDto emailDto)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailDto.From));
            email.To.Add(MailboxAddress.Parse(emailDto.To));
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailDto.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, MailKit.Security.SecureSocketOptions.Auto, CancellationToken.None);
            smtp.Authenticate(_configuration.GetSection("EmailUserName").Value, _configuration.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}


