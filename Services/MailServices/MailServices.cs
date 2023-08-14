using MailKit.Net.Smtp;

namespace API_Test1.Services
{
    public class MailServices : IMailServices
    {
        public  MessageStatus SendMail(string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("anhtuan1990nxht@gmail.com"));
            email.To.Add(MailboxAddress.Parse("anhtuan1990nxht@gmail.com"));
            email.Subject = "Verify Register Account!";
            email.Body = new TextPart(TextFormat.Html) { Text= body};

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.server.address", 587, false);
            // Note: only needed if the SMTP server requires authentication
            smtp.Authenticate("anhtuan1990nxht@gmail.com", "ndxxwlvknuetgkqg");

            smtp.Send(email);
            smtp.Disconnect(true);
            return MessageStatus.Success;
        }
    }
}
