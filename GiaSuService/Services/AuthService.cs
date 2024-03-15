using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Security.Cryptography;
namespace GiaSuService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepo;
        public AuthService(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public async Task<bool> CreateAccount(Account account)
        {
            var isSucced = await _accountRepo.Create(account);
            return isSucced;
        }

        public async Task<Account> GetAccountById(int id)
        {
            Account account = await _accountRepo.GetById(id);
            return account;
        }

        public async Task<IEnumerable<Account>> GetAccountsByRole(string role)
        {
            int? roleId = await _accountRepo.GetRoleId(role);
            if(roleId == null)
            {
                return null!;
            }

            return await _accountRepo.GetAccountsByRoleId((int)roleId);
        }

        public async Task<int?> GetRoleId(string roleName)
        {
            return await _accountRepo.GetRoleId(roleName);
        }

        public async Task<bool> UpdateAccount(Account account)
        {
            var isSucced = await _accountRepo.Update(account);
            return isSucced;
        }

        public async Task<Account> ValidateAccount(string email, string password)
        {
            Account account = await _accountRepo.GetByEmail(email);
            if (account == null)
            {
                return null!;
            }

            if(BCrypt.Net.BCrypt.Verify(password, account.Passwordhash))
            {
                return account;
            }

            return null!;
        }
    }
}
