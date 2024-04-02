﻿using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface IProfileRepo
    {
        public Task<int?> GetProfileId(int accountId, string roleName);
        public Task<Identitycard?> GetIdentitycard(string identityNumber);

        public Task<List<AccountListViewModel>> GetEmployeeList(int crrPage);
        public Task<ProfileViewModel?> GetEmployeeProfile(int empId);
        public Task<TutorProfileViewModel?> GetTutorProfile(int tutorId);
        public Task<bool> UpdateEmployeProfile(ProfileViewModel employeeProfile);

    }
}
