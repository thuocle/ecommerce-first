namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailControllers : ControllerBase
    {
        private readonly IMailServices _mailServices;

        public MailControllers(IMailServices mailServices)
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
