using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITransactionhistoryRepository : IRepository<Transactionhistory>
    {
        public Task<Transactionhistory?> GetById(int id);

        public Task<IEnumerable<Transactionhistory>> GetByAccountPayId(int accountPayId);

        public Task<IEnumerable<Transactionhistory>> GetAll();
    }
}
