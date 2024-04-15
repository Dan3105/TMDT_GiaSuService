using GiaSuService.Configs;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ITransactionService
    {
        public Task<TutorTransactionDetailViewModel> GetDetailTutorQueueTransaction(int tutorId, int requestId);
        public Task<ResponseService> CreateRefundService(int tutorId, int requestId, int empId);

        public Task<IEnumerable<TransactionDetailViewModel>> GetListTutorTransaction(int tutorId);
    }
}
