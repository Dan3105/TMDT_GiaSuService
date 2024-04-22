using GiaSuService.Models.UtilityViewModel;
using System.Data;

namespace GiaSuService.Repository.Interface
{
    public interface IStatisticRepo
    {
        public Task<AccountStatisticsViewModel?> GetAccountsCount(); 
        Task<DataTable?> GetProfit(DateOnly fromDate, DateOnly toDate);
        public Task<DataTable?> QueryStatisticAccount(int roleId, DateOnly fromDate, DateOnly toDate);
        Task<DataTable?> QueryStatisticRequestsCreate(DateOnly fromDate, DateOnly toDate);
        Task<TutorRequestStatisticsViewModel> QueryStatisticRequests(DateOnly fromDate, DateOnly toDate, int topK);

    }
}
