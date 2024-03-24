using GiaSuService.EntityModel;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Diagnostics;
using System.Security.Principal;

namespace GiaSuService.Services
{
    public class ConfigpricehistoryService : IConfigpricehistoryService
    {
        private readonly IConfigpricehistoryRepository _configRepository;
        public ConfigpricehistoryService(IConfigpricehistoryRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public async Task<Configpricehistory?> GetConfigpricehistoryById(int id)
        {
            Configpricehistory? config = (await _configRepository.GetById(id));
            return config;
        }

        public async Task<List<Configpricehistory>> GetConfigpricehistories()
        {
            List<Configpricehistory> configs = (await _configRepository.GetAll()).ToList();
            return configs;
        }

        public async Task<List<Configpricehistory>> GetConfigpricehistoriesAfterDate(DateTime fromDate)
        {
            List<Configpricehistory> configs = (await _configRepository.GetAfterDate(fromDate)).ToList();
            return configs;
        }

        public async Task<bool> CreateConfigpricehistory(Configpricehistory config)
        {
            try
            {
                _configRepository.Create(config);
                var isSucced = await _configRepository.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateConfigpricehistory(Configpricehistory config)
        {
            try
            {
                _configRepository.Update(config);
                var isSucced = await _configRepository.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
