using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IConfigpriceRepository : IRepository<Configprice>
    {
        public Task<Configprice?> GetById(int id);

        public Task<IEnumerable<Configprice>> GetAll();

        public Task<IEnumerable<Configprice>> GetAfterDate(DateTime dateTime);

    }
}
