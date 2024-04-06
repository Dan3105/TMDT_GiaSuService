using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface IProfileRepo
    {
        public Task<int?> GetProfileId(int accountId, string roleName);
        public Task<IdentityCard?> GetIdentitycard(string identityNumber);

        public Task<List<AccountListViewModel>> GetEmployeeList(int crrPage);

        public Task<ProfileViewModel?> GetProfile(int profileId, string role);
        public Task<TutorProfileViewModel?> GetTutorProfile(int tutorId);
        public Task<bool> UpdateProfile(ProfileViewModel profile, string role);
        public Task<bool> UpdateTutorProfile(TutorProfileViewModel profile);

    }
}
