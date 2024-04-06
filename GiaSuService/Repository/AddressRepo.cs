using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class AddressRepo : IAddressRepo
    {
        private readonly DvgsDbContext _context;
        public AddressRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DistrictViewModel>> GetAllDistricts(int provinceId)
        {
            return await _context.Districts
                .AsNoTracking()
                .Where(p => p.ProvinceId == provinceId)
                .Select(p => new DistrictViewModel
                {
                    DistrictName = p.Name,
                    DistrictId = p.Id
                })
                .OrderBy(p => p.DistrictName.Length)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProvinceViewModel>> GetAllProvinces()
        {
            return await _context.Provinces
                .AsNoTracking()
                .Select(p => new ProvinceViewModel
                {
                    ProvinceId = p.Id,
                    ProvinceName = p.Name,
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateAddress(int accountId, string addressDetail, int districtId)
        {
            Account? account = await _context.Accounts.FirstOrDefaultAsync(p => p.Id == accountId);
            if(account != null)
            {
                //account.Addressdetail = addressDetail;
                //account.Districtid = districtId;

                _context.Accounts.Update(account);
                return await SaveChanges();
            }

            return false;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<DistrictViewModel?> GetDistrict(int districtId)
        {
            DistrictViewModel? district = (await _context.Districts
                .Include(p => p.Province)
                .Select(p => new DistrictViewModel
                {
                    DistrictId = districtId,
                    DistrictName = p.Name,
                })
                .FirstOrDefaultAsync(p=>p.DistrictId == districtId));

            return district;
        }
    }
}
