using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface IAddressRepo
    {
        public Task<bool> UpdateAddress(int accountId, string detailAddress, int districtId);
        public Task<bool> SaveChanges();

        public Task<IEnumerable<DistrictViewModel>> GetAllDistricts(int provinceId);
        public Task<IEnumerable<ProvinceViewModel>> GetAllProvinces();
        public Task<DistrictViewModel?> GetDistrict(int districtId);

    }
}
