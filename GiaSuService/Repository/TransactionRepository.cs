using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TmdtDvgsContext _context;

        public TransactionRepository(TmdtDvgsContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetById(int id)
        {
            return await _context.Transactions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetByAccountPayId(int accountPayId)
        {
            return (await _context.Transactions
                .Where(p => p.Accountpayid == accountPayId)
                .ToListAsync());
        }

        public async Task<IEnumerable<Transaction>> GetAll()
        {
            return await _context.Transactions.ToListAsync();
        }

        public void Create(Transaction entity)
        {
            _context.Transactions.Add(entity);
        }

        public void Update(Transaction entity)
        {
            _context.Transactions.Update(entity);
        }

        public void Delete(Transaction entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
