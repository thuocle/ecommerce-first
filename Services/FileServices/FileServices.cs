using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace API_Test1.Services.FileServices
{
    public class FileServices : IFileServices
    {
        private readonly string[] Scopes = { DriveService.Scope.Drive };
        private readonly string ApplicationName = "food-upload";
        private readonly string CredentialsFilePath = "client_secret.json";
        private readonly string TokenDirectoryPath = "token.json";
        private readonly string DestinationFolderId = "1fhR03GJIhxvX8rL1z9hkXWYiXHpPZPt6";

        private DriveService _driveService;

        public FileServices()
        {
            UserCredential credential;

            using (var stream = new FileStream(CredentialsFilePath, FileMode.Open, FileAccess.Read))
            {
                string credPath = TokenDirectoryPath;
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            var fileMetadata = new File
            {
                Name = file.FileName,
                Parents = new List<string> { DestinationFolderId }
            };

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "webViewLink";

                var uploadProgress = await request.UploadAsync();

                if (uploadProgress.Status == UploadStatus.Completed)
                {
                    var uploadedFile = request.ResponseBody;
                    return uploadedFile.WebViewLink;
                }
                else
                {
                    // Xử lý lỗi tải lên tại đây nếu cần thiết
                    return null;
                }
            }
        }
    }
}