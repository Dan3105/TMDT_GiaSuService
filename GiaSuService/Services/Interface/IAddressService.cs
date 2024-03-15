using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface IAddressService
    {
        public Task<List<District>> GetDistricts(int provinceId);  
        public Task<List<Province>> GetProvinces();
        public Task<bool> UpdateAddress(int accountId, string addressName, int districtId);
        public Task<District> GetDistrictData(int districtId);
    }
}
