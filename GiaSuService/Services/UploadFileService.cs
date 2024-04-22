using Firebase.Auth;
using Firebase.Storage;
using GiaSuService.Configs;
using GiaSuService.Services.Interface;
using System.Net.Sockets;

namespace GiaSuService.Services
{
    public class UploadFileService : IUploadFileService
    {
        private static string ApiKey = "AIzaSyAX0i9bcqvtDuSZkMCvocxEefshoWoUJ9A";
        private static string Bucket = "giasuproject-imagestorage.appspot.com";
        private static string AuthEmail = "superadmin@gmail.com";
        private static string AuthPassword = "superadmin";

        private static string AVATAR_FOLDER_NAME = "user-avatar";
        private static string IDENTITY_CARD_FOLDER_NAME = "user-identity_card";

        private string GetFolderName(AppConfig.UploadFileType type)
        {
            if (type == AppConfig.UploadFileType.AVATAR) return AVATAR_FOLDER_NAME;

            if (type == AppConfig.UploadFileType.FRONT_IDENTITY_CARD ||
                type == AppConfig.UploadFileType.BACK_IDENTITY_CARD) return IDENTITY_CARD_FOLDER_NAME;

            return string.Empty;
        }

        public async Task<ResponseService> UploadFile(IFormFile file, AppConfig.UploadFileType type)
        {
            ResponseService service = new ResponseService() { Success = false, Message = "Upload ảnh thất bại" };
            if (file != null && file.Length > 0)
            {
                try
                {
                    // Get the file name and extension
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);

                    // Generate a unique file name
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    var folderName = GetFolderName(type);

                    #region Case: Upload file on firebase storage (enable)
                    string imageUrl = await UploadToFirebaseStorage(file, folderName, uniqueFileName);
                    service.Success = true;
                    service.Message = imageUrl;
                    #endregion

                    #region Case: Upload file directly (disable)
                    /*// Combine the target folder path with the unique file name
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to the target path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }*/
                    #endregion
                }
                catch
                { }
            }
            return service;
        }

        private async Task<string> UploadToFirebaseStorage(IFormFile file, string folderName, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new CancellationTokenSource();

            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0; // Reset stream position before uploading

            // Upload image to firebase
            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(folderName)
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            string imageUrl = string.Empty;
            try
            {
                imageUrl = await task;
                // Use imageUrl as needed (e.g., save to database, display in UI)
                //Console.WriteLine($"Image uploaded successfully. URL: {imageUrl}");
            }
            catch // (Exception ex) //use for testing on console
            {
                //Console.WriteLine($"Error uploading image: {ex.Message}");
            }

            return imageUrl;
        }
    }
}
