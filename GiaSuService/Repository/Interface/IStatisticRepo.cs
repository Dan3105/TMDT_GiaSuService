using GiaSuService.Models.UtilityViewModel;
using System.Data;

namespace GiaSuService.Repository.Interface
{
    public interface IStatisticRepo
    {
        public Task<AccountStatisticsViewModel?> GetAccountsCount(); 
        public Task<AccountRegisterStatisticsViewModel?> QueryStatisticAccount(string typeDate, DateOnly fromDate, DateOnly toDate);

        Task<TutorRequestStatisticCreateViewModel?> QueryStatisticRequestsCreate(string type, DateOnly from, DateOnly to);
        Task<TutorRequestStatisticsViewModel> QueryStatisticRequests();
        
        Task<TransactionStatisticsViewModel?> QueryStatisticTransactions();
        Task<TransactionStatisticByDateViewModel?> QueryChartTransactionsByDate(string typeDate, DateOnly fromDate, DateOnly toDate);
    }
}
