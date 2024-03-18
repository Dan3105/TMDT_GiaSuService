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

        public async Task<IEnumerable<District>> GetAllDistricts(int idProvince)
        {
            return (await _context.Districts.Where(p => p.Provinceid == idProvince).ToListAsync());
        }

        public async Task<IEnumerable<Province>> GetAllProvinces()
        {
            return await _context.Provinces.ToListAsync();
        }

        public async Task<bool> UpdateAddress(int accountId, string addressDetail, int districtId)
        {
            Account? account = await _context.Accounts.FirstOrDefaultAsync(p => p.Id == accountId);
            if(account != null)
            {
                account.Addressdetail = addressDetail;
                account.Districtid = districtId;

                _context.Accounts.Update(account);
                return await SaveChanges();
            }

            return false;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<District> GetDistrict(int districtId)
        {
            District district = (await _context.Districts
                .FindAsync(districtId))!;

            return district;
        }
    }
}
