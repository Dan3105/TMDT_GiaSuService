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

        public Task<bool> CreateAddress(AddressViewModel address)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DistrictViewModel>> GetDistricts(int idProvince)
        {
            IEnumerable<District> districts = await _addressRepository.GetAllDistricts(idProvince);
            List<DistrictViewModel> result = new List<DistrictViewModel>();
            foreach(District district in districts)
            {
                result.Add(new DistrictViewModel
                {
                    DistrictId = district.Id,
                    DistrictName = district.Districtname,
                    ProvinceId = district.Provinceid,
                });
            }

            return result;
        }

        public async Task<List<ProvinceViewModel>> GetProvinces()
        {
            IEnumerable<Province> provinces = await _addressRepository.GetAllProvinces();
            List<ProvinceViewModel> result = new List<ProvinceViewModel>();
            foreach (Province province in provinces)
            {
                result.Add(new ProvinceViewModel
                {
                    ProvinceId= province.Id,
                    ProvinceName = province.Provincename,
                });
            }

            return result;
        }

        public Task<bool> UpdateAddress(AddressViewModel address)
        {
            throw new NotImplementedException();
        }
    }
}
