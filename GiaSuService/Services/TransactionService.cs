using GiaSuService.Configs;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepo _transactionRepo;

        public TransactionService(ITransactionRepo transactionRepo, IProfileRepo profileRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<ResponseService> CreateRefundService(int tutorId, int requestId, int empId)
        {
            var depositTransactionExits = await _transactionRepo.GetTransactionDetailByTutorAndRequest(tutorId, requestId, true);
            if(depositTransactionExits == null)
            {
                return new ResponseService { Message = "Không tìm thấy hóa đơn nhận lớp để tạo một hóa đơn hoàn trả", Success = false };
            }

            if (string.IsNullOrEmpty(depositTransactionExits.PaymentDate))
            {
                return new ResponseService { Message = "Hóa đơn nhận lớp chưa được thanh toán", Success = false };
            }

            var refundTransactionExits = await _transactionRepo.GetTransactionDetailByTutorAndRequest(tutorId, requestId, false);
            if (refundTransactionExits != null)
            {
                return new ResponseService { Message = "Hóa đơn hoàn trả này đã tồn tại", Success = false };
            }

            bool isSuccess = await _transactionRepo.CreateRefundTransaction(tutorId, requestId, empId);
            if(isSuccess)
            {
                return new ResponseService { Message = "Tạo hóa đơn thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success=false };
        }

        public async Task<TutorTransactionDetailViewModel> GetDetailTutorQueueTransaction(int tutorId, int requestId)
        {
            var depositTransaction = await _transactionRepo.GetTransactionDetailByTutorAndRequest(tutorId, requestId, true);
            var refundTransaction = await _transactionRepo.GetTransactionDetailByTutorAndRequest(tutorId, requestId, false);

            var result = new TutorTransactionDetailViewModel
            {
                TransactionDeposit = depositTransaction,
                TransactionRefund = refundTransaction,
            };

            return result;
        }

        public async Task<IEnumerable<TransactionDetailViewModel>> GetListTutorTransaction(int tutorId)
        {
            return await _transactionRepo.GetTransactionsTutor(tutorId);
        }
    }
}
