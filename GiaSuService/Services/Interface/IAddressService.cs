using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface IAddressService
    {
        public Task<List<DistrictViewModel>> GetDistricts(int idProvince);  
        public Task<List<ProvinceViewModel>> GetProvinces();
        public Task<bool> UpdateAddress(int accountId, string addressName, int districtId);

    }
}
