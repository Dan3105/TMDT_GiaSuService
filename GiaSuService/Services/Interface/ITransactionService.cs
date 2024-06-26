﻿using GiaSuService.Configs;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ITransactionService
    {
        public Task<TransactionDetailViewModel?> GetTransactionDetail(int transactionId);
        public Task<TutorTransactionDetailViewModel> GetDetailTutorQueueTransaction(int tutorId, int requestId);
        public Task<ResponseService> CreateRefundTransaction(int tutorId, int requestId, int empId);

        public Task<ResponseService> UpdateDepositPaymentTransaction(int tutorId, int requestId, DateTime paymentDate);
        public Task<IEnumerable<TransactionDetailViewModel>> GetListTutorTransaction(int tutorId);


        public Task<PageTransactionListViewModel> GetListTransaction(
            AppConfig.TransactionFilterStatus payStatus,
            AppConfig.TransactionFilterType transactionType,
            int currPage);
    }
}
