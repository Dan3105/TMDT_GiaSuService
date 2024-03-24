using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transRepository;
        public TransactionService(ITransactionRepository transRepository)
        {
            _transRepository = transRepository;
        }

        public async Task<Transaction?> GetTransactionById(int id)
        {
            Transaction? trans = (await _transRepository.GetById(id));
            return trans;
        }

        public async Task<List<Transaction>> GetTransactionByAccountPayId(int accountPayId)
        {
            List<Transaction> trans = (await _transRepository.GetByAccountPayId(accountPayId)).ToList();
            return trans;
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            List<Transaction> trans = (await _transRepository.GetAll()).ToList();
            return trans;
        }

        public async Task<bool> CreateTransaction(Transaction trans)
        {
            try
            {
                _transRepository.Create(trans);
                var isSucced = await _transRepository.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTransaction(Transaction trans)
        {
            try
            {
                _transRepository.Update(trans);
                var isSucced = await _transRepository.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
