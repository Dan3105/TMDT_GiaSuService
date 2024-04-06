using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface IConfigpriceService
    {
        public Task<ConfigPrice?> GetConfigpriceById(int id);
        public Task<List<ConfigPrice>> GetConfigpricehistories();
        public Task<List<ConfigPrice>> GetConfigpricehistoriesAfterDate(DateTime datetime);
        public Task<bool> CreateConfigprice(ConfigPrice config);
        public Task<bool> UpdateConfigprice(ConfigPrice config);
    }
}
