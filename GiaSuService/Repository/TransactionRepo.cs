
using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly DvgsDbContext _context;
        public TransactionRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateRefundTransaction(int tutorId, int requestId, int empId)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var originAmount = await _context.TransactionHistories
                        .Select(p => new { p.TutorId, p.FormId, p.TypeTransaction, p.PaymentAmount, p.PaymentDate })
                        .FirstOrDefaultAsync(t => t.TutorId == tutorId && t.TypeTransaction && t.FormId == requestId);

                    if (originAmount == null || !originAmount.PaymentDate.HasValue) { throw new NullReferenceException(); }

                    var tutor = await _context.Tutors.AsNoTracking().Select(p => new { p.Id, p.FullName }).FirstOrDefaultAsync(t => t.Id == tutorId);
                    if(tutor == null) { throw new NullReferenceException(); }

                    var emp = await _context.Employees.AsNoTracking().Select(p => new { p.Id, p.FullName, p.Account.Phone }).FirstOrDefaultAsync(t => t.Id == empId);
                    if(emp == null) { throw new NullReferenceException(); }

                    var statusPaid = await _context.Statuses.FirstOrDefaultAsync(p => p.Name.Equals(AppConfig.TransactionStatus.PAID.ToString().ToLower())
                                                                                            && p.StatusType.Type.ToLower().Equals(AppConfig.transaction_status.ToLower()));
                    if (statusPaid == null)
                    {
                        throw new NullReferenceException();
                    }

                    _context.TransactionHistories.Add(new EntityModel.TransactionHistory
                    {
                        CreateDate = DateTime.Now,
                        PaymentAmount = originAmount.PaymentAmount,
                        EmployeeId = emp.Id,
                        TutorId = tutorId,
                        Context = AppConfig.ContextForRefundTransaction(tutor.FullName, emp.Phone, emp.FullName, originAmount.PaymentAmount),
                        FormId = requestId,
                        PaymentDate = DateTime.Now,
                        TypeTransaction = false,
                        StatusId = statusPaid.Id,
                    });

                    var statusRefund = await _context.Statuses
                        .Select(p => new {p.Name, p.Id, p.StatusType.Type})
                        .FirstOrDefaultAsync(t => t.Name.Equals(AppConfig.QueueStatus.REFUND.ToString().ToLower()) && t.Type.Equals(AppConfig.queue_status));
                    if(statusRefund == null) { throw new NullReferenceException(); }

                    await _context.TutorApplyForms.Where(p => p.TutorId == tutorId && p.TutorRequestId == requestId)
                            .ExecuteUpdateAsync(builder => builder.SetProperty(p => p.StatusId, statusRefund.Id));

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<TransactionDetailViewModel?> GetTransactionDetailByTutorAndRequest(int tutorId, int requestId, bool isDeposit)
        {
            var result = (await _context.TransactionHistories.AsNoTracking()
                .Select(p => new
                {
                    Transaction = new TransactionDetailViewModel
                    {
                        TransactionId = p.Id,
                        TutorId = p.TutorId,
                        RequestId = p.FormId,

                        EmployeeName = p.Employee.FullName,
                        TutorName = p.Tutor.FullName,

                        CreateDate = p.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"),
                        PaymentDate = ((DateTime)p.PaymentDate!).ToString("HH:mm:ss dd/MM/yyyy") ?? string.Empty,
                        Price = p.PaymentAmount,

                        Context = p.Context ?? string.Empty,
                    },
                    p.TypeTransaction
                })
                .FirstOrDefaultAsync(p => p.Transaction.TutorId == tutorId && p.Transaction.RequestId == requestId && p.TypeTransaction == isDeposit)
                )?.Transaction;

            if (result == null) return result;

            if (isDeposit)
            {
                var tutorRequest = await _context.RequestTutorForms
                .AsNoTracking()
                .Include(p => p.Status)
                .Select(p => new { p.Id, p.Status.Name })
                .FirstOrDefaultAsync(p => p.Id == requestId);

                var tutorApply = await _context.TutorApplyForms
                .AsNoTracking()
                .Include(p => p.Status)
                .Select(p => new { p.TutorId, p.TutorRequestId, p.Status.Name })
                .FirstOrDefaultAsync(p => p.TutorId == tutorId && p.TutorRequestId == requestId);

                result.RequestStatus = tutorRequest != null ? tutorRequest.Name : string.Empty;
                result.QueueStatus = tutorApply != null ? tutorApply.Name : string.Empty;
            }
            
            return result;
        }

        public async Task<IEnumerable<TransactionDetailViewModel>> GetTransactionsTutor(int tutorid)
        {
            return await _context.TransactionHistories.AsNoTracking()
                .Select(p => new{ Transaction=new TransactionDetailViewModel
                {
                    Context = p.Context ?? string.Empty,
                    CreateDate = p.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"),
                    PaymentDate = ((DateTime)p.PaymentDate!).ToString("HH:mm:ss dd/MM/yyyy") ?? string.Empty,
                    Price = p.PaymentAmount,
                    EmployeeName = p.Employee.FullName,
                    TutorName = p.Tutor.FullName,
                    TutorId = p.TutorId,
                    RequestId = p.FormId
                },
                p.CreateDate
                })
                .OrderByDescending(p => p.CreateDate)
                .Where(p => p.Transaction.TutorId == tutorid)
                .Select(p => p.Transaction)
                .ToListAsync();
        }

        public async Task<bool> UpdateDepositTransactionPaymentDate(int tutorId, int requestId, DateTime paydate)
        {
            try {

                var statusPaid = await _context.Statuses
                    .FirstOrDefaultAsync(p => p.Name.Equals(AppConfig.TransactionStatus.PAID.ToString().ToLower())
                                    && p.StatusType.Type.ToLower().Equals(AppConfig.transaction_status.ToLower()));
                
                if (statusPaid == null)
                {
                    throw new NullReferenceException();
                }

                await _context.TransactionHistories.
                    Where(p => p.TypeTransaction && p.TutorId == tutorId && p.FormId == requestId)
                    .ExecuteUpdateAsync(p =>
                        p.SetProperty(o => o.PaymentDate, paydate)
                         .SetProperty(o => o.StatusId, statusPaid.Id)
                    );

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<PageTransactionListViewModel> GetListTransaction(
            AppConfig.TransactionFilterStatus payStatus,
            AppConfig.TransactionFilterType transactionType,
            int currPage)
        {
            var result = new PageTransactionListViewModel();

            // Get list transaction
            var query = _context.TransactionHistories
                .AsNoTracking()
                .Select(p => new
                {
                    Transaction = new TransactionCardViewModel
                    {
                        TransactionId = p.Id,
                        CreateDate = p.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"),
                        Price = p.PaymentAmount,
                        EmployeeName = p.Employee.FullName,
                        TransactionType = (p.TypeTransaction ? AppConfig.TransactionFilterType.PAID.ToString() 
                                                                : AppConfig.TransactionFilterType.REFUND.ToString()),
                        PayStatus = p.PaymentDate != null ? AppConfig.TransactionFilterStatus.PAID.ToString() :
                                                            AppConfig.TransactionFilterStatus.UNPAID.ToString(),
                    },
                    p.CreateDate,
                    p.PaymentDate
                })
                .OrderByDescending(p => p.CreateDate)
                .Select(p => p.Transaction)
                ;

            // Filter transaction status
            if (payStatus == AppConfig.TransactionFilterStatus.PAID)
            {
                query = query.Where(p => p.PayStatus == AppConfig.TransactionFilterStatus.PAID.ToString());
            }
            else if (payStatus == AppConfig.TransactionFilterStatus.UNPAID)
            {
                query = query.Where(p => p.PayStatus == AppConfig.TransactionFilterStatus.UNPAID.ToString());
            }

            // Filter transaction type
            if (transactionType == AppConfig.TransactionFilterType.PAID)
            {
                query = query.Where(p => p.TransactionType == AppConfig.TransactionFilterType.PAID.ToString());
            }
            else if (transactionType == AppConfig.TransactionFilterType.REFUND)
            {
                query = query.Where(p => p.TransactionType == AppConfig.TransactionFilterType.REFUND.ToString());
            }

            result.TotalElement = await query.CountAsync();
            result.list = await query.Skip(currPage * AppConfig.ROWS_ACCOUNT_LIST)
                        .Take(AppConfig.ROWS_ACCOUNT_LIST).ToListAsync();

            return result;
        }


        public async Task<TransactionDetailViewModel?> GetTransactionDetail(int transactionId)
        {
            var result = (await _context.TransactionHistories.AsNoTracking()
                .Select(p => new
                {
                    Transaction = new TransactionDetailViewModel
                    {
                        TransactionId = p.Id,
                        TutorId = p.TutorId,
                        RequestId = p.FormId,

                        EmployeeName = p.Employee.FullName,
                        TutorName = p.Tutor.FullName,

                        CreateDate = p.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"),
                        PaymentDate = ((DateTime)p.PaymentDate!).ToString("HH:mm:ss dd/MM/yyyy") ?? string.Empty,
                        Price = p.PaymentAmount,

                        Context = p.Context ?? string.Empty,

                        TransactionType = (p.TypeTransaction ? AppConfig.TransactionFilterType.PAID.ToString()
                                                                : AppConfig.TransactionFilterType.REFUND.ToString()),
                    },
                    p.Id
                })
                .FirstOrDefaultAsync(p => p.Id == transactionId)
                )?.Transaction;

            return result;
        }
    }
}
