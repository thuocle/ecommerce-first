using API_Test1.Models.DTOs;

namespace API_Test1.Services.MailServices
{
    public interface IMailServices
    {
        public MessageStatus SendMail(MailDTOs request);
    }
}
