﻿using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface IProfileRepo
    {
        public Task<int?> GetProfileId(int accountId, string roleName);
        public Task<IdentityCard?> GetIdentitycard(string identityNumber);

        public Task<List<AccountListViewModel>> GetEmployeeList(int crrPage);
        public Task<int> GetCountEmployeeList();

        public Task<ProfileViewModel?> GetProfile(int profileId, string role);
        public Task<TutorProfileViewModel?> GetTutorProfile(int tutorId);
        public Task<TutorFormUpdateProfileViewModel?> GetTutorFormUpdateProfile(int tutorId);
        public Task<bool> UpdateProfile(ProfileViewModel profile, string role);
        public Task<bool> UpdateAvatar(int accountId, string imageUrl);
        public Task<bool> UpdateRequestTutorProfile(TutorFormUpdateProfileViewModel original, TutorFormUpdateProfileViewModel modified);

        public Task<bool> UpdateActiveTutor(TutorFormUpdateProfileViewModel modified, TutorFormUpdateProfileViewModel exitsTutor);

        public Task<DifferenceUpdateRequestFormViewModel?> GetTutorsDifferenceProfile(int historyId);
    }
}
