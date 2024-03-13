using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IAddressRepository
    {
        public Task<bool> UpdateAddress(int accountId, string detailAddress, int districtId);
        public Task<bool> SaveChanges();

        public Task<IEnumerable<District>> GetAllDistricts(int idProvince);
        public Task<IEnumerable<Province>> GetAllProvinces();

    }
}
