﻿using GiaSuService.Models.UtilityViewModel;
using System.Data;

namespace GiaSuService.Services.Interface
{
    public interface IStatisticService
    {
        public Task<AccountRegisterStatisticsViewModel?> GetAccountCreateStatistic(string type, DateOnly fromDate, DateOnly toDate);
        Task<AccountStatisticsViewModel?> GetStatisticAccount();

        Task<TutorRequestStatisticCreateViewModel?> GetRequestCreated(string type, DateOnly fromDate, DateOnly toDate);
        Task<TutorRequestStatisticsViewModel> GetStatisticRequest();

        Task<TransactionStatisticByDateViewModel?> GetTransactionCreated(string type, DateOnly fromDate, DateOnly toDate);
        Task<TransactionStatisticsViewModel?> GetStatisticTranssaction();
    }
}
