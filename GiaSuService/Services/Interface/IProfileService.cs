﻿using GiaSuService.Configs;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Services.Interface
{
    public interface IProfileService
    {
        // Use this function to get employeeId, customerId or tutorId by accountId
        public Task<int?> GetProfileId(int accountId, string roleName);

        public Task<List<AccountListViewModel>> GetEmployeeList(int page);
        public Task<int> GetCountEmployeeList();
        
        // This GetProfile function just use to get profile of userRole (customer or employee)
        // param profileId is customerId or employeeId base on userRole
        public Task<ProfileViewModel?> GetProfile(int profileId, string userRole);
        public Task<ResponseService> UpdateProfile(ProfileViewModel model, IFormFile avatar, IFormFile front, IFormFile back, string userRole);
        
        public Task<TutorProfileViewModel?> GetTutorProfile(int tutorId);
        public Task<ResponseService> UpdateTutorProfileInEmployee(TutorProfileViewModel model);

        public Task<TutorFormUpdateProfileViewModel?> GetTutorFormUpdateById(int tutorId);
        public Task<ResponseService> CreateRequestTutorProfile(TutorUpdateRequestViewModel model, IFormFile avatar, IFormFile frontCard, IFormFile backCard);
    }
}
