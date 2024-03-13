using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly TmdtDvgsContext _context;
        public AddressRepository(TmdtDvgsContext context)
        {
            _context = context;
        }

        public IQueryable<Address> All()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Create(Address entity)
        {
            try
            {
                _context.Addresses.Add(entity);
                return await SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<District>> GetAllDistricts(int idProvince)
        {
            return (await _context.Districts.Where(p => p.Provinceid == idProvince).ToListAsync());
        }

        public async Task<IEnumerable<Province>> GetAllProvinces()
        {
            return await _context.Provinces.ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public Task<bool> Update(Address entity)
        {
            throw new NotImplementedException();
        }
    }
}
