using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface IAddressService
    {
        public Task<List<DistrictViewModel>> GetDistricts(int provinceId);  
        public Task<List<ProvinceViewModel>> GetProvinces();
        public Task<bool> UpdateAddress(int accountId, string addressName, int districtId);
        public Task<DistrictViewModel?> GetDistrictData(int districtId);
    }
}
