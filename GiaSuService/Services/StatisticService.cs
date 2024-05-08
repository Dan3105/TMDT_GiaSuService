using GiaSuService.Configs;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Data;

namespace GiaSuService.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatusRepo _statusRepo;
        private readonly IStatisticRepo _statisticRepo;
        public StatisticService( IStatisticRepo statisticRepo, IStatusRepo statusRepo)
        {
            _statisticRepo = statisticRepo;
            _statusRepo = statusRepo;
        }

        public async Task<AccountRegisterStatisticsViewModel?> GetAccountCreateStatistic(string type, DateOnly fromDate, DateOnly toDate)
        {
            return await _statisticRepo.QueryStatisticAccount(type, fromDate, toDate);
        }

        public async Task<AccountStatisticsViewModel?> GetStatisticAccount()
        {
            return await _statisticRepo.GetAccountsCount();
        }

        public async Task<DataTable?> GetRequestCreated(DateOnly fromDate, DateOnly toDate)
        {
            return await _statisticRepo.QueryStatisticRequestsCreate(fromDate, toDate);
        }

        public async Task<TutorRequestStatisticsViewModel> GetStatisticRequest(DateOnly fromDate, DateOnly toDate, int topK)
        {
            var result = await _statisticRepo.QueryStatisticRequests(fromDate, toDate, topK);
            return result;
        }

        public async Task<DataTable?> GetTransactionCreated(DateOnly fromDate, DateOnly toDate)
        {
            return await _statisticRepo.QueryChartDataTransactions(fromDate, toDate);
        }

        public async Task<TransactionStatisticsViewModel> GetStatisticTranssaction(DateOnly fromDate, DateOnly toDate)
        {
            return await _statisticRepo.QueryStatisticTransactions(fromDate, toDate);
        }
    }
}
