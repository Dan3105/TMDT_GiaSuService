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

        public ProfileService(IProfileRepo profileRepo)
        {
            _profileRepo = profileRepo;
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
    }
}