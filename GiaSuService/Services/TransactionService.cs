using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TransactionhistoryService : ITransactionhistoryService
    {
        private readonly ITransactionhistoryRepository _transRepository;
        //public TransactionhistoryService(ITransactionhistoryRepository transRepository)
        //{
        //    _transRepository = transRepository;
        //}

        public async Task<Transactionhistory?> GetTransactionhistoryById(int id)
        {
            Transactionhistory? trans = (await _transRepository.GetById(id));
            return trans;
        }

        public async Task<List<Transactionhistory>> GetTransactionhistoryByAccountPayId(int accountPayId)
        {
            List<Transactionhistory> trans = (await _transRepository.GetByAccountPayId(accountPayId)).ToList();
            return trans;
        }

        public async Task<List<Transactionhistory>> GetTransactionhistorys()
        {
            List<Transactionhistory> trans = (await _transRepository.GetAll()).ToList();
            return trans;
        }

        public async Task<bool> CreateTransactionhistory(Transactionhistory trans)
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

        public async Task<bool> UpdateTransactionhistory(Transactionhistory trans)
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
