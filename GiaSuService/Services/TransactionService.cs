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
        private readonly ITutorRequestFormService _tutorRequestService;

        public TransactionService(ITransactionRepo transactionRepo, IProfileRepo profileRepo,
            ITutorRequestFormService tutorRequestService)
        {
            _transactionRepo = transactionRepo;
            _tutorRequestService = tutorRequestService;
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

            // Check if tutor request is currently not handover then cannot refund to tutor
            var tutorRequest = await _tutorRequestService.GetTutorRequestFormById(requestId);
            if (tutorRequest == null)
            {
                return new ResponseService { Message = "Không tìm thấy đơn tìm gia sư", Success = false };
            }

            if (tutorRequest.CurrentStatus.ToLower() != AppConfig.FormStatus.HANDOVER.ToString().ToLower())
            {
                return new ResponseService { Message = "Đơn tìm gia sư chưa được giao cho gia sư này nên không thể hoàn tiền", Success = false };
            }

            // Create refund transaction
            bool isSuccess = await _transactionRepo.CreateRefundTransaction(tutorId, requestId, empId);

            // Update Tutor Request status from handover to approve
            ResponseService response = await _tutorRequestService.UpdateStatusTutorRequest(requestId, AppConfig.FormStatus.APPROVAL.ToString());
            if (!response.Success) isSuccess = false;


            // Update Tutor Apply status from handover to deny
            response = await _tutorRequestService.UpdateStatusTutorApply(tutorId, requestId,
                AppConfig.QueueStatus.REFUND.ToString());
            if (!response.Success) isSuccess = false;

            if (isSuccess)
            {
                return new ResponseService { Message = "Tạo hóa đơn hoàn tiền thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
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
                bool createAndPay = DateTime.Compare(createTimeConvert, paymentDate) >= 0;
                //bool payAndNow = DateTime.Compare(paymentDate, DateTime.Now) > 0;
                
                // Check paymentDate
                if(createAndPay)
                {
                    return new ResponseService { Message = "Ngày trả tiền nhỏ hơn ngày tạo hóa đơn", Success = false };
                }

                //if (payAndNow)
                //{
                //    return new ResponseService { Message = "Ngày trả tiền lớn hơn ngày hôm nay", Success = false };
                //}

                bool success = true;

                // Check if tutor request is currently handover then cannot handover to new tutor
                var tutorRequest = await _tutorRequestService.GetTutorRequestFormById(requestId);
                if(tutorRequest == null)
                {
                    return new ResponseService { Message = "Không tìm thấy đơn tìm gia sư", Success = false };
                }

                if (tutorRequest.CurrentStatus.ToLower() == AppConfig.FormStatus.HANDOVER.ToString().ToLower())
                {
                    return new ResponseService { Message = "Đơn tìm gia sư đã được giao", Success = false };
                }

                // Update tutorRequestForm status to handover
                ResponseService response = await _tutorRequestService.UpdateStatusTutorRequest(requestId, AppConfig.FormStatus.HANDOVER.ToString());
                if (!response.Success) success = false;

                // Update paymentDate on transaction
                success = await _transactionRepo.UpdateDepositTransactionPaymentDate(tutorId, requestId, paymentDate);

                // Update tutorApplyForm status to handover 
                response = await _tutorRequestService.UpdateStatusTutorApply(tutorId, requestId,
                    AppConfig.QueueStatus.HANDOVER.ToString());
                if (!response.Success) success = false;


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

        public async Task<TransactionDetailViewModel?> GetTransactionDetail(int transactionId)
        {
            return await _transactionRepo.GetTransactionDetail(transactionId);
        }
    }
}
