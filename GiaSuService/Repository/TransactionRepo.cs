
using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
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

                    _context.TransactionHistories.Add(new EntityModel.TransactionHistory
                    {
                        CreateDate = DateTime.Now,
                        PaymentAmount = originAmount.PaymentAmount,
                        EmployeeId = emp.Id,
                        TutorId = tutorId,
                        Context = AppConfig.ContextForRefundTransaction(tutor.FullName, emp.Phone, emp.Phone, originAmount.PaymentAmount),
                        FormId = requestId,
                        PaymentDate = DateTime.Now,
                        TypeTransaction = false
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
            return (await _context.TransactionHistories.AsNoTracking()
                .Select(p => new
                {
                    Transaction = new TransactionDetailViewModel
                    {
                        Context = p.Context ?? string.Empty,
                        CreateDate = p.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"),
                        PaymentDate = ((DateTime)p.PaymentDate!).ToString("HH:mm:ss dd/MM/yyyy") ?? string.Empty,
                        Price = p.PaymentAmount,
                        EmployeeName = p.Employee.FullName,
                        TutorName = p.Tutor.FullName,
                        TransactionId = p.Id,
                        TutorId = p.TutorId,
                        RequestId = p.FormId
                    },
                    p.TypeTransaction
                })
                .FirstOrDefaultAsync(p => p.Transaction.TutorId == tutorId && p.Transaction.RequestId == requestId && p.TypeTransaction == isDeposit)
                )?.Transaction;
            //throw new NotImplementedException();
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
                    TransactionId = p.Id,
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
    }
}
