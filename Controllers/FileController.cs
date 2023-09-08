using API_Test1.Services.FileServices;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileServices _fileServices;

        public FileController(IFileServices fileServices)
        {
            _fileServices = fileServices;
        }
        /*[HttpPost("upload")]
        public async Task<IActionResult> UpLoad(IFormFile file)
        {
            return Ok(await _fileServices.UploadImage(file));
        }*/
    }
}
