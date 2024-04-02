using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Diagnostics;

namespace GiaSuService.Services
{
    public class TutorRequestFormService : ITutorRequestFormService
    {
        private readonly DvgsDbContext _context;
        private readonly ITutorRequestRepo _tutorRequestRepo;
        private readonly IStatusRepo _statusRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IQueueRepo _queueRepo;

        public TutorRequestFormService(DvgsDbContext context, ITutorRequestRepo tutorRequestRepo, IStatusRepo statusRepo, ICategoryRepo categoryRepo
            , IQueueRepo queueRepo)
        {
            _context = context;
            _tutorRequestRepo = tutorRequestRepo;
            _statusRepo = statusRepo;
            _categoryRepo = categoryRepo;
            _queueRepo = queueRepo;
        }

        public async Task<Tutorrequestform?> GetTutorRequestFormById(int formId)
        {
            Tutorrequestform? form = (await _tutorRequestRepo.Get(formId));
            return form;
        }

        public async Task<ResponseService> CreateForm(Tutorrequestform form, List<int> listSession, List<int> listTutor)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Status? status = await _statusRepo.GetStatus(AppConfig.FormStatus.PENDING.ToString(), AppConfig.form_status);
                    if (status == null)
                    {
                        throw new NullReferenceException();
                    }
                    form.Status = status;
                    _tutorRequestRepo.Create(form);
                    await _tutorRequestRepo.SaveChanges();

                    if (!listSession.Any())
                    {
                        return new ResponseService { Success = false, Message = "Bạn chưa chọn ngày dạy" };
                    }
                    var sessionQueries = _context.Sessiondates.Where(p => listSession.Contains(p.Id));
                    foreach(var session in sessionQueries)
                    {
                        form.Sessions.Add(session);
                    }
                    if (listTutor.Count > 0)
                    {
                        int? statusId = (await _statusRepo.GetStatus(AppConfig.QueueStatus.APPROVAL.ToString(), AppConfig.queue_status))?.Id;
                        if (statusId == null)
                        {
                            return new ResponseService { Success = false, Message = "Việc chọn gia sư không có lỗi vui lòng làm lại" };
                        }
                        bool isSuccess = _queueRepo.AddTutorsToQueue(form, listTutor, (int)statusId);
                        await _tutorRequestRepo.SaveChanges();
                    }
                    
                    
                    _tutorRequestRepo.Update(form);
                    await _tutorRequestRepo.SaveChanges();
                    transaction.Commit();
                    return new ResponseService { Success = true, Message = "Tạo đơn yêu cầu thành công" };
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new ResponseService { Success = false, Message = "Lỗi hệ thống" };
                }
            }
           
        }

        public async Task<bool> UpdateForm(Tutorrequestform form, List<int> sessionList, string statusName)
        {
            try
            {
                Status? status = await _statusRepo.GetStatus(statusName, AppConfig.form_status);
                if (status == null)
                {
                    throw new NullReferenceException();
                }
                form.Status = status;
                _tutorRequestRepo.Update(form);
                var isSucced = await _tutorRequestRepo.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public async Task<List<Tutorrequestform>> GetTutorrequestforms(AppConfig.TutorRequestStatus status)
        //{
        //    return await _repo.GetByStatus(status);
        //}
    }
}
