using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GiaSuService.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IAuthService _authService;
        private readonly IProfileRepo _profileRepo;
        private readonly ITutorRepo _tutorRepo;
        private readonly IUploadFileService _uploadFileService;

        public ProfileService(IAuthService authService, IProfileRepo profileRepo, ITutorRepo tutorRepo, IUploadFileService uploadFileService)
        {
            _authService = authService;
            _profileRepo = profileRepo;
            _tutorRepo = tutorRepo;
            _uploadFileService = uploadFileService;
        }

        public async Task<int?> GetProfileId(int accountId, string roleName)
        {
            return await _profileRepo.GetProfileId(accountId, roleName);
        }

        public Task<List<AccountListViewModel>> GetEmployeeList(int page)
        {
            return _profileRepo.GetEmployeeList(page);
        }

        public async Task<ProfileViewModel?> GetProfile(int profileId, string userRole)
        {
            return await _profileRepo.GetProfile(profileId, userRole);
        }

        public async Task<ResponseService> UpdateProfile(ProfileViewModel profile, IFormFile avatar, IFormFile front, IFormFile back, string userRole)
        {
            if (profile == null || userRole == null || userRole == "")
            {
                return new ResponseService { Success = false, Message = "Lỗi cập nhật" };
            }

            #region Check_if_change_email_or_phone_then_check_exist
            
            /*ResponseService rs = await _authService.CheckEmailExist(profile.Email);
            if (rs.Success)
            {
                return new ResponseService { Success = false, Message = "Email đã được sử dụng. Vui lòng thử email khác" };
            }

            rs = await _authService.CheckEmailExist(profile.Phone);
            if (rs.Success)
            {
                return new ResponseService { Success = false, Message = "Số điện thoại đã được sử dụng. Vui lòng thử số điện thoại khác" };
            }*/
            #endregion

            #region Upload_image
            if (avatar != null)
            {
                ResponseService response = await _uploadFileService.UploadFile(avatar, AppConfig.UploadFileType.AVATAR);
                if (!response.Success) return response;
                profile.Avatar = response.Message;
            }

            if (front != null)
            {
                ResponseService response = await _uploadFileService.UploadFile(front, AppConfig.UploadFileType.FRONT_IDENTITY_CARD);
                if (!response.Success) return response;
                profile.FrontIdentityCard = response.Message;
            }

            if (back != null)
            {
                ResponseService response = await _uploadFileService.UploadFile(back, AppConfig.UploadFileType.BACK_IDENTITY_CARD);
                if (!response.Success) return response;
                profile.BackIdentityCard = response.Message;
            }
            #endregion

            bool isSuccess = await _profileRepo.UpdateProfile(profile, userRole);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Cập nhật thành công" };
            }
            return new ResponseService { Success = false, Message = "Cập nhật thất bại" };
        }

        public async Task<TutorProfileViewModel?> GetTutorProfile(int profileId)
        {
            return await _profileRepo.GetTutorProfile(profileId);
        }

        public async Task<ResponseService> CreateRequestTutorProfile(TutorUpdateRequestViewModel profile, IFormFile avatar, IFormFile frontCard, IFormFile backCard)
        {
            profile.Form.SelectedDistricts = profile.DistrictSelected;
            profile.Form.SelectedGradeIds = profile.GradeSelected;
            profile.Form.SelectedSessionIds = profile.SessionSelected;
            profile.Form.SelectedSubjectIds = profile.SubjectSelected;

            var origin = await _profileRepo.GetTutorFormUpdateProfile(profile.Form.TutorId);
            if(origin == null)
            {
                return new ResponseService { Success = true, Message = "Không tìm thấy thông tin gia sư trong hệ thống" }; 
            }

            if (avatar != null)
            {
                ResponseService response = await _uploadFileService.UploadFile(avatar, AppConfig.UploadFileType.AVATAR);
                if (!response.Success) return response;
                profile.Form.Avatar = response.Message;
            }

            if (frontCard != null)
            {
                ResponseService response = await _uploadFileService.UploadFile(frontCard, AppConfig.UploadFileType.FRONT_IDENTITY_CARD);
                if (!response.Success) return response;
                profile.Form.FrontIdentityCard = response.Message;
            }

            if (backCard != null)
            {
                ResponseService response = await _uploadFileService.UploadFile(backCard, AppConfig.UploadFileType.BACK_IDENTITY_CARD);
                if (!response.Success) return response;
                profile.Form.BackIdentityCard = response.Message;
            }

            TutorFormUpdateProfileViewModel? diff = TutorFormUpdateProfileViewModel.CompareDifference(origin, profile.Form);
            if(diff == null)
            {
                return new ResponseService { Success = false, Message = "Bạn chưa thay đổi thông tin nào" };
            }
            bool isSuccess = await _profileRepo.UpdateRequestTutorProfile(origin, profile.Form);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Cập nhật thành công" };
            }
            return new ResponseService { Success = false, Message = "Cập nhật thất bại" };
        }


        //This code only update lockenable and identitynumber
        public async Task<ResponseService> UpdateTutorProfileInEmployee(TutorProfileViewModel model)
        {
            Tutor? tutorProfile = await _tutorRepo.GetTutor(model.TutorId);
            if(tutorProfile == null)
            {
                return new ResponseService { Success = false, Message = "Không tìm được mã nhân viên" };
            }

            IdentityCard? identitycard = await _profileRepo.GetIdentitycard(model.IdentityCard);
            
            if(identitycard != null && tutorProfile.Identity.IdentityNumber != identitycard.IdentityNumber)
            {
                return new ResponseService { Success = false, Message = "Chứng minh thư đã tồn tại trong hệ thống" };
            }

            tutorProfile.Account.LockEnable = model.LockEnable;
            tutorProfile.Identity.IdentityNumber = model.IdentityCard;

            var isSuccess = await _tutorRepo.UpdateTutor(tutorProfile);
            if (isSuccess) { return new ResponseService { Success = true, Message = "Cập nhật gia sư thành công" }; }
            return new ResponseService { Success = false, Message = "Lỗi cập nhật trong hệ thống" };
            
        }

        public Task<TutorFormUpdateProfileViewModel?> GetTutorFormUpdateById(int tutorId)
        {
            return _profileRepo.GetTutorFormUpdateProfile(tutorId);
        }

        public Task<int> GetCountEmployeeList()
        {
            return _profileRepo.GetCountEmployeeList();
        }
    }
}