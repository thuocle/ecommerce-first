using API_Test1.Migrations;
using API_Test1.Services.PaymentServices.MOMO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private readonly IMoMoServices _moMoServices;

        public PayController(IMoMoServices moMoServices)
        {
            _moMoServices = moMoServices;
        }
    }
}
