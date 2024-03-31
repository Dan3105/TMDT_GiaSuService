using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITransactionhistoryService
    {
        public Task<Transactionhistory?> GetTransactionhistoryById(int id);
        public Task<List<Transactionhistory>> GetTransactionhistoryByAccountPayId(int accountPayId);
        public Task<List<Transactionhistory>> GetTransactionhistorys();
        public Task<bool> CreateTransactionhistory(Transactionhistory trans);
        public Task<bool> UpdateTransactionhistory(Transactionhistory trans);
    }
}
