using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface IProfileService
    {
        public Task<int?> GetIdProfile(int accountId, string roleName);

        public Task<List<AccountListViewModel>> GetEmployeeList(int page);
        
        // Function to get or update user with role employee or customer
        public Task<ProfileViewModel?> GetProfile(int accountId, string userRole);
        public Task<ResponseService> UpdateProfile(ProfileViewModel profile, string userRole);

        // Function to get or update tutor
        public Task<TutorProfileViewModel?> GetTutorProfile(int accountId);
        public Task<ResponseService> UpdateTutorProfile(TutorProfileViewModel profile);

    }
}
