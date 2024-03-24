using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IConfigpricehistoryRepository : IRepository<Configpricehistory>
    {
        public Task<Configpricehistory?> GetById(int id);

        public Task<IEnumerable<Configpricehistory>> GetAll();

        public Task<IEnumerable<Configpricehistory>> GetAfterDate(DateTime dateTime);

    }
}
