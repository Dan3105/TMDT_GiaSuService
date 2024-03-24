using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITransactionService
    {
        public Task<Transaction?> GetTransactionById(int id);
        public Task<List<Transaction>> GetTransactionByAccountPayId(int accountPayId);
        public Task<List<Transaction>> GetTransactions();
        public Task<bool> CreateTransaction(Transaction trans);
        public Task<bool> UpdateTransaction(Transaction trans);
    }
}
