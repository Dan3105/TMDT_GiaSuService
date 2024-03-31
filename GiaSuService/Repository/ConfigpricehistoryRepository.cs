using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class ConfigpriceRepository : IConfigpriceRepository
    {
        private readonly DvgsDbContext _context;

        public ConfigpriceRepository(DvgsDbContext context)
        {
            _context = context;
        }

        public void Create(Configprice entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Configprice entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Configprice>> GetAfterDate(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Configprice>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Configprice?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Update(Configprice entity)
        {
            throw new NotImplementedException();
        }
    }
}
