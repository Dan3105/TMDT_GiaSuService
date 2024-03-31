using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class TransactionhistoryRepository : ITransactionhistoryRepository
    {
        private readonly DvgsDbContext _context;

        public TransactionhistoryRepository(DvgsDbContext context)
        {
            _context = context;
        }

        public void Create(Transactionhistory entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Transactionhistory entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transactionhistory>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transactionhistory>> GetByAccountPayId(int accountPayId)
        {
            throw new NotImplementedException();
        }

        public Task<Transactionhistory?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Update(Transactionhistory entity)
        {
            throw new NotImplementedException();
        }
    }
}
