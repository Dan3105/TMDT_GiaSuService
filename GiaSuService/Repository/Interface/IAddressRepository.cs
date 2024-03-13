using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IAddressRepository : IRepository<Address>
    {
        public Task<IEnumerable<District>> GetAllDistricts(int idProvince);
        public Task<IEnumerable<Province>> GetAllProvinces();
    }
}
