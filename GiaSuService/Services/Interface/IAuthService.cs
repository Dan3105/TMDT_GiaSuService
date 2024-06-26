﻿using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Services.Interface
{
    public interface IAuthService
    {
        public Task<ResponseService> CheckEmailExist(string email);
        public Task<ResponseService> CheckPhoneExist(string phone);
        public Task<Account?> GetAccountById(int id);
        public Task<Account?> ValidateAccount(string email, string password);
        public Task<ResponseService> CreateAccount(Account account);
        //Use this for customer or employee account
        public Task<ResponseService> CreateAccount(RegisterAccountProfileViewModel? profile, IFormFile avatar, IFormFile frontCard, IFormFile backCard, string accountRole);
        public Task<ResponseService> CreateTutorAccount(FormRegisterTutorRequestViewModel? profile, IFormFile avatar, IFormFile frontCard, IFormFile backCard);
        public Task<ResponseService> UpdateAccount(Account account);
        public Task<ResponseService> UpdatePassword(int accountId, string password);
        public Task<ResponseService> CreateTutorRegisterRequest(Account account, IEnumerable<int> sessionIds, IEnumerable<int> subjectIds, IEnumerable<int> gradeIds,
            IEnumerable<int> districtIds);
        public Task<IEnumerable<Account>> GetAccountsByRole(string role);
        public Task<int?> GetRoleId(string roleName);
    }
}
