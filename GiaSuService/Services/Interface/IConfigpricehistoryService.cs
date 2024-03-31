using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface IConfigpriceService
    {
        public Task<Configprice?> GetConfigpriceById(int id);
        public Task<List<Configprice>> GetConfigpricehistories();
        public Task<List<Configprice>> GetConfigpricehistoriesAfterDate(DateTime datetime);
        public Task<bool> CreateConfigprice(Configprice config);
        public Task<bool> UpdateConfigprice(Configprice config);
    }
}
