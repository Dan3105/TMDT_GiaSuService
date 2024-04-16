using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Principal;

namespace GiaSuService.Repository
{
    public class QueueRepo : IQueueRepo
    {
        private readonly DvgsDbContext _context;
        public QueueRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public bool AddTutorsToQueue(RequestTutorForm form, List<int> ids, int statusDefaultId)
        {
            try {
                form.TutorApplyForms = new List<TutorApplyForm>();
                foreach(var id in ids)
                {
                    form.TutorApplyForms.Add(
                    new TutorApplyForm
                    {
                        EnterDate = DateTime.Now,
                        StatusId = statusDefaultId,
                        TutorId = id,
                    });
                }

                _context.Update(form);
                return true;
            }
            catch (Exception) {
                return false;
            }
           
        }

        public async Task<bool> AddTutorToQueue(int tutorId, int requestId, int statusId)
        {
            try
            {
                TutorApplyForm form = new TutorApplyForm
                {
                    TutorId = tutorId,
                    TutorRequestId = requestId,
                    EnterDate = DateTime.Now,
                    StatusId = statusId,
                };

                _context.Add(form);
                return await SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CancelApplyRequest(int tutorId, int requestId, int statusId)
        {
            try
            {
                TutorApplyForm? form = await _context.TutorApplyForms.FirstOrDefaultAsync(p => p.TutorRequestId == requestId && p.TutorId == tutorId);
                if (form == null)
                {
                    return false;
                }

                form.StatusId = statusId;
                _context.Update(form);
                return await SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<TutorCardViewModel>> GetTutorInQueueByForm(int requestId, int statusId = 0)
        {
            var queries = await _context.TutorApplyForms
                .Select(p => new
                {
                    Tutor = new TutorCardViewModel
                    {
                        Id = p.TutorId,
                        Avatar = p.Tutor.Account.Avatar,
                        FullName = p.Tutor.FullName,
                        Area = p.Tutor.Area,
                        College = p.Tutor.College,
                        TutorType = p.Tutor.TutorType.Name,
                    },
                    p.TutorRequestId,
                    p.StatusId
                })
                .Where(p => p.TutorRequestId == requestId && (statusId == 0 || statusId == p.StatusId))
                .Select(p => p.Tutor)
                .ToListAsync();
                ;

            return queries;
        }

        public async Task<List<TutorApplyRequestQueueViewModel>> GetTutorsApplyRequestQueue(int requestId)
        {
            var queries = await _context.TutorApplyForms
                .Select(p => new
                {
                    Tutor = new TutorApplyRequestQueueViewModel
                    {
                        Avatar = p.Tutor.Account.Avatar,
                        FullName = p.Tutor.FullName,
                        StatusQueue = p.Status.Name,
                        TutorId = p.TutorId,
                        TutorType = p.Tutor.TutorType.Name,
                    },
                    p.TutorRequestId,
                    p.EnterDate
                })
                .OrderBy(p => p.EnterDate)
                .Where(p => p.TutorRequestId == requestId)
                .Select(p => p.Tutor)
                .ToListAsync();
            ;

            foreach(var record in queries)
            {
                bool any = await _context.TransactionHistories.AnyAsync(p=> p.TutorId == record.TutorId && p.FormId == requestId);
                record.IsHaveTransaction = any;
            }

            return queries;
        }

        public async Task<bool> UpdateTutorQueue(int requestId, int tutorId, Status status, int employeeId)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var tutorQueue = await _context.TutorApplyForms
                                .Select(p => new {p.StatusId, p.TutorRequest.GradeId, p.TutorRequestId, p.TutorId})
                                .FirstOrDefaultAsync(p => p.TutorId == tutorId && p.TutorRequestId == requestId);
                    
                    if(tutorQueue == null)
                    {
                        throw new NullReferenceException();
                    }

                    if(tutorQueue.StatusId == status.Id)
                    {
                        return true;
                    }

                    if (status.Name.Equals(AppConfig.QueueStatus.APPROVAL.ToString().ToLower()))
                    {
                        var price = (await _context.Grades.FindAsync(tutorQueue.GradeId))?.Fee;
                        if (price == null)
                        {
                            throw new NullReferenceException();
                        }

                        var employeeProfile = await _context.Employees.Select(p => new {p.FullName, p.Id, p.Account.Phone}).FirstOrDefaultAsync(p => p.Id == employeeId);
                        if (employeeProfile == null)
                        {
                            throw new NullReferenceException();
                        }

                        TransactionHistory newTransaction = new TransactionHistory()
                        {
                            CreateDate = DateTime.Now,
                            TutorId = tutorId,
                            Context = AppConfig.ContextForApplyTutor(employeeProfile.FullName, employeeProfile.Phone),
                            EmployeeId = employeeId,
                            PaymentAmount = (decimal)price,
                            FormId = requestId,
                            TypeTransaction = AppConfig.DEPOSIT_TYPE
                        };

                        _context.TransactionHistories.Add(newTransaction);
                        //throw new NotImplementedException();
                        //await _context.SaveChangesAsync();
                    }

                    if (status.Name.Equals(AppConfig.QueueStatus.HANDOVER.ToString().ToLower()))
                    {
                        var statusType = await _context.StatusTypes.FirstOrDefaultAsync(p => p.Type.ToLower().Equals(AppConfig.form_status.ToLower()));
                        if (statusType == null) { throw new NullReferenceException(); }
                        var formStatus = await _context.Statuses.FirstOrDefaultAsync(p => p.Name.ToLower().Equals(AppConfig.FormStatus.HANDOVER.ToString().ToLower())
                                                                                && p.StatusTypeId==statusType.Id);
                        if (formStatus == null) { throw new NullReferenceException(); }

                        await _context.RequestTutorForms
                            .Where(p => p.Id == requestId)
                            .ExecuteUpdateAsync(p => p.SetProperty(p => p.StatusId, formStatus.Id));
                            
                    }

                    await _context.TutorApplyForms
                    .Where(p => p.TutorId == tutorId && p.TutorRequestId == requestId)
                    .ExecuteUpdateAsync(builder => builder.SetProperty(p => p.StatusId, status.Id));

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch(Exception) {
                    transaction.Rollback();
                    return false;
                }
            }
                            
        }
    }
}
