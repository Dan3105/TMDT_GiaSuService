using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepo _profileRepo;
        private readonly ITutorRepo _tutorRepo;

        public ProfileService(IProfileRepo profileRepo, ITutorRepo tutorRepo)
        {
            _profileRepo = profileRepo;
            _tutorRepo = tutorRepo;
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

        // Kiểm tra tài khoản thay đổi email, sdt, cccd có đc hay ko?
        /*private async Task<ResponseService> CheckDuplicated(string email, string phone, string identitycard)
        {

            return false;
        }*/

        public async Task<ResponseService> UpdateProfile(ProfileViewModel profile, string userRole)
        {
            if (profile == null || userRole == null || userRole == "")
            {
                return new ResponseService { Success = false, Message = "Lỗi cập nhật" };
            }

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

        public async Task<ResponseService> CreateRequestTutorProfile(TutorUpdateRequestViewModel profile)
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
            TutorFormUpdateProfileViewModel? diff = TutorFormUpdateProfileViewModel.CompareDifference(origin, profile.Form);
            bool isSuccess = await _profileRepo.UpdateRequestTutorProfile(diff);
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
    }
}