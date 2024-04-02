using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface IProfileService
    {
        public Task<int?> GetIdProfile(int accountId, string roleName);

        public Task<List<AccountListViewModel>> GetEmployeeList(int page);
        public Task<ProfileViewModel?> GetEmployeeProfile(int empId);

        public Task<ResponseService> UpdateEmployeeProfile(ProfileViewModel model);
    }
}
