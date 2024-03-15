using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IAccountRepository : IRepository<Account>
    {
        public Task<Account> GetByEmail(string email);
        public Task<Account> GetById(int id);
        public Task<int?> GetRoleId(string roleName);
        public Task<IEnumerable<Account>> GetAccountsByRoleId(int roleId);
    }
}
