using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class ConfigpricehistoryRepository : IConfigpricehistoryRepository
    {
        private readonly TmdtDvgsContext _context;

        public ConfigpricehistoryRepository(TmdtDvgsContext context)
        {
            _context = context;
        }

        public async Task<Configpricehistory?> GetById(int id)
        {
            return await _context.Configpricehistories.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Configpricehistory>> GetAll()
        {
            return await _context.Configpricehistories.ToListAsync();
        }

        public async Task<IEnumerable<Configpricehistory>> GetAfterDate(DateTime dateTime)
        {
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);

            var configs = await _context.Configpricehistories
            .Where(c => c.Createdate.HasValue && c.Createdate.Value >= dateOnly)
            .ToListAsync();

            return configs;
        }

        public void Create(Configpricehistory entity)
        {
            _context.Configpricehistories.Add(entity);
        }

        public void Update(Configpricehistory entity)
        {
            _context.Configpricehistories.Update(entity);
        }

        public void Delete(Configpricehistory entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

    }
}
