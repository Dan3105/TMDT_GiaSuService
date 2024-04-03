using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface IProfileRepo
    {
        public Task<int?> GetProfileId(int accountId, string roleName);
        public Task<Identitycard?> GetIdentitycard(string identityNumber);

        public Task<List<AccountListViewModel>> GetEmployeeList(int crrPage);


        public Task<ProfileViewModel?> GetEmployeeProfile(int accountId);
        public Task<ProfileViewModel?> GetCustomerProfile(int accountId);
        public Task<TutorProfileViewModel?> GetTutorProfile(int accountId);
        public Task<ResponseService> UpdateEmployeeProfile(ProfileViewModel profile);
        public Task<ResponseService> UpdateCustomerProfile(ProfileViewModel profile);
        public Task<ResponseService> UpdateTutorProfile(TutorProfileViewModel profile);

    }
}
