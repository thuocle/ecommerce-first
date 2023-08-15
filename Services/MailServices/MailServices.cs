
using API_Test1.Models.DTOs;

namespace API_Test1.Services.MailServices
{
    public class MailServices : IMailServices
    {
        private readonly IConfiguration _configuration;

        public MailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public MessageStatus SendMail(MailDTOs request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("GmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration["GmailHost"], 587, false);
            // Note: only needed if the SMTP server requires authentication
            smtp.Authenticate(
                _configuration["GmailUserName"], _configuration["GmailPassWord"]);
            smtp.Send(email);
            smtp.Disconnect(true);
            return MessageStatus.Success;
        }
    }
}
