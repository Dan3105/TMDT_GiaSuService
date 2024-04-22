using GiaSuService.Models.UtilityViewModel;
using System.Data;

namespace GiaSuService.Services.Interface
{
    public interface IStatisticService
    {
        public Task<DataTable?> GetAccountCreateStatistic(int roleId, DateOnly fromDate, DateOnly toDate);
        Task<AccountStatisticsViewModel?> GetStatisticAccount();
        Task<DataTable?> GetRequestCreated(DateOnly fromDate, DateOnly toDate);
        Task<TutorRequestStatisticsViewModel> GetStatisticRequest(DateOnly fromDate, DateOnly toDate, int topK);
    }
}
