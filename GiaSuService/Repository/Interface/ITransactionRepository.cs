using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        public Task<Transaction?> GetById(int id);

        public Task<IEnumerable<Transaction>> GetByAccountPayId(int accountPayId);

        public Task<IEnumerable<Transaction>> GetAll();
    }
}
