using GiaSuService.Configs;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;

namespace GiaSuService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepo _transactionRepo;

        public TransactionService(ITransactionRepo transactionRepo, IProfileRepo profileRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<ResponseService> CreateRefundTransaction(int tutorId, int requestId, int empId)
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

        public async Task<ResponseService> UpdateDepositPaymentTransaction(int tutorId, int requestId, DateTime paymentDate)
        {
            var depositTransactionExits = await _transactionRepo.GetTransactionDetailByTutorAndRequest(tutorId, requestId, true);
            if (depositTransactionExits == null)
            {
                return new ResponseService { Message = "Không tìm thấy hóa đơn nhận lớp để tạo một hóa đơn hoàn trả", Success = false };
            }
            try
            {
                DateTime createTimeConvert = DateTime.ParseExact(depositTransactionExits.CreateDate, "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture);
                bool createAndPay = DateTime.Compare(createTimeConvert, paymentDate) > 0;
                bool payAndNow = DateTime.Compare(paymentDate, DateTime.Now) > 0;
                if(createAndPay)
                {
                    return new ResponseService { Message = "Ngày trả tiền nhỏ hơn ngày tạo hóa đơn", Success = false };
                }

                if (payAndNow)
                {
                    return new ResponseService { Message = "Ngày trả tiền lớn hơn ngày hôm nay", Success = false };
                }


                bool success = await _transactionRepo.UpdateDepositTransactionPaymentDate(tutorId, requestId, paymentDate);
                if (success)
                {
                    return new ResponseService { Message = "Hệ thống cập nhật thành công", Success =true };
                }
                    
                return new ResponseService { Message = "Hệ thống cập nhật thất bại", Success =false };
            }
            catch (FormatException) {
                return new ResponseService { Message = "Format ngày không chính xác", Success = false };
            }
        }

        public async Task<PageTransactionListViewModel> GetListTransaction(
            AppConfig.TransactionFilterStatus payStatus,
            AppConfig.TransactionFilterType transactionType,
            int currPage)
        {
            return await _transactionRepo.GetListTransaction(payStatus, transactionType, currPage);
        }
    }
}
