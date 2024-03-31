using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface IAuthService
    {
        public Task<Account> GetAccountById(int id);
        public Task<Account> ValidateAccount(string email, string password);
        public Task<bool> CreateAccount(Account account);
        public Task<bool> UpdateAccount(Account account);
        public Task<bool> CreateTutorRegisterRequest(Account account, Tutor tutorprofile, IEnumerable<int> districtId, IEnumerable<int> gradeId, IEnumerable<int> subjectId,
            IEnumerable<int> sessionId);
        public Task<IEnumerable<Account>> GetAccountsByRole(string role);

        public Task<int?> GetRoleId(string roleName);
    }
}
