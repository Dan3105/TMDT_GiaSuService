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

        public Account CreateAccount(Account account)
        {
            throw new NotImplementedException();
        }

        public Account GetAccountByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Account> GetAllAccountsByRole(string role)
        {
            throw new NotImplementedException();
        }

        public Account UpdateAccount(Account account)
        {
            throw new NotImplementedException();
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
