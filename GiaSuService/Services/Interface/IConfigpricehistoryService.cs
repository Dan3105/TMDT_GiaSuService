using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface IConfigpricehistoryService
    {
        public Task<Configpricehistory?> GetConfigpricehistoryById(int id);
        public Task<List<Configpricehistory>> GetConfigpricehistories();
        public Task<List<Configpricehistory>> GetConfigpricehistoriesAfterDate(DateTime datetime);
        public Task<bool> CreateConfigpricehistory(Configpricehistory config);
        public Task<bool> UpdateConfigpricehistory(Configpricehistory config);
    }
}
