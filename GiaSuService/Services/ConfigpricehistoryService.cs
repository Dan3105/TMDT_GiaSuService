using GiaSuService.EntityModel;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Diagnostics;
using System.Security.Principal;

namespace GiaSuService.Services
{
    public class ConfigpriceService : IConfigpriceService
    {
        private readonly IConfigpriceRepo _configRepository;
        //public ConfigpriceService(IConfigpriceRepository configRepository)
        //{
        //    _configRepository = configRepository;
        //}

        public async Task<Configprice?> GetConfigpriceById(int id)
        {
            Configprice? config = (await _configRepository.GetById(id));
            return config;
        }

        public async Task<List<Configprice>> GetConfigpricehistories()
        {
            List<Configprice> configs = (await _configRepository.GetAll()).ToList();
            return configs;
        }

        public async Task<List<Configprice>> GetConfigpricehistoriesAfterDate(DateTime fromDate)
        {
            List<Configprice> configs = (await _configRepository.GetAfterDate(fromDate)).ToList();
            return configs;
        }

        public async Task<bool> CreateConfigprice(Configprice config)
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

        public async Task<bool> UpdateConfigprice(Configprice config)
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
