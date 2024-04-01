using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IAccountRepo : IRepository<Account>
    {
        public Task<Account?> GetByEmailOrPhone(string loginname);
        public Task<Account?> GetById(int id);
        public Task<int?> GetRoleId(string roleName);
        public Task<IEnumerable<Account>> GetAccountsByRoleId(int roleId);

    }
}
