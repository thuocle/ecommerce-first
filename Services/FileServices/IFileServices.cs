namespace API_Test1.Services.FileServices
{
    public interface IFileServices
    {
        /* public Task<string> UploadFileToGoogleDrive(IFormFile file , string folderPath);*/
        public Task<string> UploadImage(IFormFile file);
    }
}
