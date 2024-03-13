using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface IAddressService
    {
        public Task<List<DistrictViewModel>> GetDistricts(int idProvince);  
        public Task<List<ProvinceViewModel>> GetProvinces();
        public Task<bool> CreateAddress(AddressViewModel address);
        public Task<bool> UpdateAddress(AddressViewModel address);

    }
}
