using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepo _addressRepository;
        public AddressService(IAddressRepo addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<DistrictViewModel?> GetDistrictData(int districtId)
        {
            DistrictViewModel? district = await _addressRepository.GetDistrict(districtId);
            return district;
        }

        public async Task<List<DistrictViewModel>> GetDistricts(int idProvince)
        {
            List<DistrictViewModel> districts = (await _addressRepository.GetAllDistricts(idProvince)).ToList();
            return districts;
        }

        public async Task<List<ProvinceViewModel>> GetProvinces()
        {
            List<ProvinceViewModel> provinces = (await _addressRepository.GetAllProvinces()).ToList();
            return provinces;
        }


        public async Task<bool> UpdateAddress(int accountId, string addressName, int districtId)
        {
            try
            {
                return await _addressRepository.UpdateAddress(accountId, addressName, districtId);
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
