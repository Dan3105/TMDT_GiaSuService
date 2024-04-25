using GiaSuService.Models.UtilityViewModel;
using System.Data;

namespace GiaSuService.Repository.Interface
{
    public interface IStatisticRepo
    {
        public Task<AccountStatisticsViewModel?> GetAccountsCount(); 
        public Task<DataTable?> QueryStatisticAccount(int roleId, DateOnly fromDate, DateOnly toDate);
        Task<DataTable?> QueryStatisticRequestsCreate(DateOnly fromDate, DateOnly toDate);
        Task<TutorRequestStatisticsViewModel> QueryStatisticRequests(DateOnly fromDate, DateOnly toDate, int topK);
        Task<TransactionStatisticsViewModel> QueryStatisticTransactions(DateOnly fromDate, DateOnly toDate);
        Task<DataTable?> QueryChartDataTransactions(DateOnly fromDate, DateOnly toDate);
    }
}
