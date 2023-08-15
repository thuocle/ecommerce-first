using API_Test1.Models.DTOs;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailServices _mailServices;

        public MailController(IMailServices mailServices)
        {
            _mailServices = mailServices;
        }
        [HttpPost("sendMail")]
        public IActionResult SendMail(MailDTOs request) 
        {
            var result = _mailServices.SendMail(request);
            return Ok(result);
        }
    }
}
