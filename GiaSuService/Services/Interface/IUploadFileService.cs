using GiaSuService.Configs;

namespace GiaSuService.Services.Interface
{
    public interface IUploadFileService
    {
        /*
         * Upload file to firebase's storage 
         * If upload successfully return Success state and message is url to the file on firebase
         * Else return Failed state and message failed
         * */
        public Task<ResponseService> UploadFile(IFormFile file, AppConfig.UploadFileType type);
    }
}
