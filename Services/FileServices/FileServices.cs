using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace API_Test1.Services.FileServices
{
    public class FileServices : IFileServices
    {
        /*public static string[] Scopes = { DriveService.Scope.Drive };

        public static DriveService GetService()
        {
            // Đường dẫn tới tệp tin JSON của Service Account
            string serviceAccountJsonPath = @"D:\client_secret.json";

            // Đọc thông tin xác thực từ tệp tin JSON
            GoogleCredential credential;
            using (var stream = new FileStream(serviceAccountJsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes)
                    .CreateWithUser("ankiney2001@gmail.com");
            }

            // Tạo dịch vụ Drive API
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "food-upload",
            });

            return service;
        }
        private static async Task<IAuthorizationCodeFlow> GetAuthorizationFlowAsync()
        {
            string[] scopes = { DriveService.Scope.Drive };

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var clientSecrets = GoogleClientSecrets.Load(stream).Secrets;

                return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets,
                    Scopes = scopes,
                    DataStore = new FileDataStore("Drive.Api.Auth.Store")
                });
            }
        }
        // Tải lên tệp tin ảnh lên Google Drive.
        public static async Task<string> FileUploadAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var authorizationFlow = await GetAuthorizationFlowAsync();

                var credential = await new AuthorizationCodeInstalledApp(
                    authorizationFlow,
                    new LocalServerCodeReceiver()
                ).AuthorizeAsync("user", CancellationToken.None);

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "food-upload"
                });

                string fileName = Path.GetFileName(file.FileName);
                string mimeType = file.ContentType;

                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = fileName,
                    MimeType = mimeType
                };

                FilesResource.CreateMediaUpload request;

                using (var stream = file.OpenReadStream())
                {
                    request = service.Files.Create(
                        fileMetadata,
                        stream,
                        mimeType
                    );
                    request.Fields = "id";
                    await request.UploadAsync();
                }

                return request.ResponseBody?.Id;
            }

            return null;
        }*/
        
        public  async Task<UserCredential> GetUserCredential()
        {
            string[] scopes = { DriveService.ScopeConstants.DriveFile };
            var _credentialsPath = "credentials.json";

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "469648269513-9rl8h35tnifnocvs2msl2pv6lr73bhic.apps.googleusercontent.com",
                    ClientSecret = "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"
                },
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(_credentialsPath, true));

            return credential;
        }

        public  async Task<string> CreateFileOnDrive(DriveService driveService, string fileName, string folderId)
        {
            var fileMetadata = new File
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            var request = driveService.Files.Create(fileMetadata);
            request.Fields = "id";

            var createdFile = await request.ExecuteAsync();

            return createdFile.Id;
        }

        public async Task<string> UploadImageToDrive(DriveService driveService, string folderId, Stream stream, string fileName, string mimeType)
        {
            var fileMetadata = new File
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            var uploadStream = new MemoryStream();
            await stream.CopyToAsync(uploadStream);
            uploadStream.Position = 0;

            var uploadRequest = driveService.Files.Create(fileMetadata, uploadStream, mimeType);
            uploadRequest.Fields = "id";

            var progress = await uploadRequest.UploadAsync();

            if (progress.Status == UploadStatus.Completed)
            {
                return uploadRequest.ResponseBody.Id;
            }
            else
            {
                // Handle upload error
                throw new Exception("Upload failed: " + progress.Exception?.Message);
            }
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "No file uploaded.";
            }
            try
            {
                var _applicationName = "food-upload";
                var _clientId = "469648269513-9rl8h35tnifnocvs2msl2pv6lr73bhic.apps.googleusercontent.com";
                var _clientSecret = "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";
                var _accessToken = "ya29.a0AfB_byCPxxYeYi-XHwvvjPr-NploVmVDpmJ4BYh8pjeiN_iNhOMzbcUHZ3hDuEcwUy_H47dGRManGz5i3JD9DwwoN2yaP6zCaZHUFG1bFQn9g3fSYKSVOxMJlwVgXM_VIvPwk1635gbR-z9_mPb2F5l51UwfaCgYKAeQSARISFQHsvYlsAOOcPiKfAM9RuucxuMhxRg0163";
                var _refreshToken = "1//0eyYulreuDPoLCgYIARAAGA4SNwF-L9IrcalrwdrD0u1Sk9izu1dsGwwOz1J3XMLqnrJuSf0LJeD6r3LCPV26kjvWWfUQLM8Kp8Q";

                var credential = new UserCredential(
                    new GoogleAuthorizationCodeFlow(
                        new GoogleAuthorizationCodeFlow.Initializer
                        {
                            ClientSecrets = new ClientSecrets
                            {
                                ClientId = _clientId,
                                ClientSecret = _clientSecret
                            }
                        }),
                    "user",
                    new TokenResponse
                    {
                        AccessToken = _accessToken,
                        RefreshToken = _refreshToken
                    }
                );

                var driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = _applicationName
                });
                //kiem tra kieu du lieu trươc khi upload
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return "Only image files (jpg, jpeg, png, gif) are allowed.";
                }
                //upload vào thư mục driver
                string folderId = "1fhR03GJIhxvX8rL1z9hkXWYiXHpPZPt6";
                using (var stream = file.OpenReadStream())
                {
                    var uploadedFileId = await UploadImageToDrive(driveService, folderId, stream, file.FileName, file.ContentType);
                    return uploadedFileId;
                }
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}