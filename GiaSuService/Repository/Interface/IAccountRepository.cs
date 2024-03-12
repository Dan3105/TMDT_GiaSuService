using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IAccountRepository : IRepository<Account>
    {
        public Task<Account> GetByEmail(string email);
    }
}
