using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
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


        public Task<List<AccountListViewModel>> GetEmployeeList(int page)
        {
            return _profileRepo.GetEmployeeList(page);
        }

        public async Task<int?> GetIdProfile(int accountId, string roleName)
        {
            return await _profileRepo.GetProfileId(accountId, roleName);
        }

        public async Task<ProfileViewModel?> GetProfile(int accountId, string userRole)
        {
            if (userRole == AppConfig.CUSTOMERROLENAME)
            {
                return await _profileRepo.GetCustomerProfile(accountId);
            }
            return await _profileRepo.GetEmployeeProfile(accountId);

        }

        // Kiểm tra tài khoản thay đổi email, sdt, cccd có đc hay ko?
        /*private async Task<ResponseService> CheckDuplicated(string email, string phone, string identitycard)
        {

            return false;
        }*/

        public async Task<ResponseService> UpdateProfile(ProfileViewModel profile, string userRole)
        {
            ResponseService? response = null;
            if (userRole == AppConfig.CUSTOMERROLENAME)
            {
                response = await _profileRepo.UpdateCustomerProfile(profile);
            }
            else
            {
                response = await _profileRepo.UpdateEmployeeProfile(profile);
            }
            return response;
        }

        public async Task<TutorProfileViewModel?> GetTutorProfile(int accountId)
        {
            return await _profileRepo.GetTutorProfile(accountId);
        }

        public async Task<ResponseService> UpdateTutorProfile(TutorProfileViewModel profile)
        {
            return await _profileRepo.UpdateTutorProfile(profile);
        }


        //This code only update lockenable and identitynumber
        public async Task<ResponseService> UpdateTutorProfile(TutorProfileViewModel model)
        {
            Tutor? tutorProfile = await _tutorRepo.GetTutor(model.TutorId);
            if(tutorProfile == null)
            {
                return new ResponseService { Success = false, Message = "Không tìm được mã nhân viên" };
            }

            Identitycard? identitycard = await _profileRepo.GetIdentitycard(model.Identitycard);
            
            if(identitycard != null && tutorProfile.Identity.Identitynumber != identitycard.Identitynumber)
            {
                return new ResponseService { Success = false, Message = "Chứng minh thư đã tồn tại trong hệ thống" };
            }

            tutorProfile.Account.Lockenable = model.Lockenable;
            tutorProfile.Identity.Identitynumber = model.Identitycard;

            var isSuccess = await _tutorRepo.UpdateTutor(tutorProfile);
            if (isSuccess) { return new ResponseService { Success = true, Message = "Cập nhật gia sư thành công" }; }
            return new ResponseService { Success = false, Message = "Lỗi cập nhật trong hệ thống" };
            
        }
    }
}