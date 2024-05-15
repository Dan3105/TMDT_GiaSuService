using GiaSuService.Configs;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITransactionRepo
    {
        public Task<TransactionDetailViewModel?> GetTransactionDetailByTutorAndRequest(int tutorId, int requestId, bool isDeposit);
        public Task<bool> CreateRefundTransaction(int tutorId, int requestId, int empId);
        public Task<bool> UpdateDepositTransactionPaymentDate(int tutorId, int requestId, DateTime datetime);

        public Task<IEnumerable<TransactionDetailViewModel>> GetTransactionsTutor(int tutorid);


        public Task<PageTransactionListViewModel> GetListTransaction(
            AppConfig.TransactionFilterStatus payStatus,
            AppConfig.TransactionFilterType transactionType, 
            int currPage);

        public Task<TransactionDetailViewModel?> GetTransactionDetail(int transactionId);
    }
}
