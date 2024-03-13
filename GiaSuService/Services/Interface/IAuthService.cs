﻿using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface IAuthService
    {
        public Account GetAccountByEmail(string email);
        public Task<Account> ValidateAccount(string email, string password);
        public Task<bool> CreateAccount(Account account);
        public Account UpdateAccount(Account account);

        public IEnumerable<Account> GetAllAccounts();
        public IEnumerable<Account> GetAllAccountsByRole(string role);

        public Task<int?> GetRoleId(string roleName);
    }
}
