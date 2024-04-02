﻿using GiaSuService.Configs;
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

        public async Task<ProfileViewModel?> GetEmployeeProfile(int empId) { 
            return await _profileRepo.GetEmployeeProfile(empId);
        }

        public async Task<int?> GetIdProfile(int accountId, string roleName)
        {
            return await _profileRepo.GetProfileId(accountId, roleName);
        }

        public async Task<ResponseService> UpdateEmployeeProfile(ProfileViewModel model)
        {

            Identitycard? identitycard = await _profileRepo.GetIdentitycard(model.IdentityCard);
            var profileEmployee = await _profileRepo.GetEmployeeProfile(model.EmployeeId);
            if(identitycard != null && !identitycard.Identitynumber.Equals(profileEmployee?.IdentityCard))
            {
                return new ResponseService { Success = false, Message = "Chứng minh thư đã tồn tại trong hệ thống" };
            }
            bool isSuccess = await _profileRepo.UpdateEmployeProfile(model);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Cập nhật thành công" };
            }

            return new ResponseService { Success = false, Message = "Lỗi hệ thống 500" };
        }
    }
}
