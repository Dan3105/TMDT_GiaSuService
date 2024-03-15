using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<District> GetDistrictData(int districtId)
        {
            District district = await _addressRepository.GetDistrict(districtId);
            return district;
        }

        public async Task<List<District>> GetDistricts(int idProvince)
        {
            List<District> districts = (await _addressRepository.GetAllDistricts(idProvince)).ToList();
            return districts;
        }

        public async Task<List<Province>> GetProvinces()
        {
            List<Province> provinces = (await _addressRepository.GetAllProvinces()).ToList();
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
