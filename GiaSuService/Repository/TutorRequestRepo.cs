using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GiaSuService.Repository
{
    public class TutorRequestRepo : ITutorRequestRepo
    {
        private readonly DvgsDbContext _context;

        public TutorRequestRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public void Create(RequestTutorForm entity)
        {
            _context.RequestTutorForms.Add(entity);
        }

        public void Update(RequestTutorForm entity)
        {
            _context.RequestTutorForms.Update(entity);
        }

        public void Delete(RequestTutorForm entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<RequestTutorForm?> Get(int id)
        {
            return (await _context.RequestTutorForms
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.Id == id));
        }

        public async Task<List<RequestTutorForm>> GetAll()
        {
            return (await _context.RequestTutorForms.ToListAsync());
        }

        public async Task<List<RequestTutorForm>> GetByFilter(int subjectId, int gradeId, int districtId)
        {
            var filteredForms = await _context.RequestTutorForms
                .Where(p => (subjectId == 0 || p.SubjectId == subjectId)
                      && (gradeId == 0 || p.GradeId == gradeId)
                      && (districtId == 0 || p.DistrictId == districtId))
            .ToListAsync();

            return filteredForms;
        }

        public async Task<PageTutorRequestQueueListViewModel> GetTutorRequestQueueByStatus(int statusId, int page)
        {
            var query = _context.RequestTutorForms
                .AsNoTracking()
                .Select(p => new
                {
                    TutorRequest = new TutorRequestQueueViewModel
                    {
                        AddressName = $"{p.AddressDetail}, {p.District.Name}, {p.District.Province.Name}",
                        CreatedDate = ((DateTime)p.CreateDate!).ToString("dd-MM-yyyy HH:mm:ss"),
                        FormId = p.Id,
                        FullNameRequester = p.Customer.FullName,
                        GradeName = p.Grade.Name,
                        SubjectName = p.Subject.Name,
                    },
                    ExpiredDate = p.ExpiredDate,
                    CreatedDate = p.CreateDate,
                    StatusId = p.StatusId
                })
                .Where(p => p.ExpiredDate > DateTime.Now && p.StatusId == statusId)
                ;
            ;
            var result = new PageTutorRequestQueueListViewModel();
            result.TotalElement = await query.CountAsync();
            result.list = await query.OrderByDescending(p => p.CreatedDate).Skip(page * AppConfig.ROWS_ACCOUNT_LIST)
                .Take(AppConfig.ROWS_ACCOUNT_LIST)
                .Select(p => p.TutorRequest)
                .ToListAsync();

            return result;
        }

        public async Task<TutorRequestProfileViewModel?> GetTutorRequestProfile(int id)
        {
            var result = await _context.RequestTutorForms
                .AsNoTracking()
                .Select(p => new TutorRequestProfileViewModel
                {
                    FormId = p.Id,
                    CreatedDate = p.CreateDate,
                    ExpiredDate = p.ExpiredDate,
                    CurrentStatus = p.Status.VietnameseName,
                    Status = p.Status.Name,

                    FullNameRequester = p.Customer.FullName,
                    AddressName = $"{p.AddressDetail}, {p.District.Name}, {p.District.Province.Name}",
                    GradeName = p.Grade.Name,
                    SubjectName = p.Subject.Name,
                    AdditionalDetail = p.AdditionalDetail == null ? "Không có" : p.AdditionalDetail,

                }).FirstOrDefaultAsync(p => p.FormId == id);
            return result;
        }

        public async Task<PageTutorRequestListViewModel> GetTutorRequestCardByStatus(int districtId, int subjectId, int gradeId, int statusId, int page)
        {
            var result = new PageTutorRequestListViewModel { };
            var queries = _context.RequestTutorForms
                .AsNoTracking()
                .Select(p => new
                {
                    TutorCard = new TutorRequestCardViewModel
                    {
                        RequestId = p.Id,
                        GradeName = p.Grade.Name,
                        SubjectName = p.Subject.Name,
                        AdditionalDetail = p.AdditionalDetail ?? string.Empty,
                        Address = $"{p.District.Name}, {p.District.Province.Name}",
                        SessionsCanTeach = string.Join(", ", p.Sessions.Select(p => p.Name)),
                        Price = p.Grade.Fee,
                    },
                    GradeId = p.GradeId,
                    SubjectId = p.SubjectId,
                    DistrictId = p.DistrictId,
                    Status = p.StatusId,
                    Expired = p.ExpiredDate
                })
                .OrderBy(p => p.Expired)
                .Where(p => p.Status == statusId && p.Expired > DateTime.Now
                        && (districtId == 0 || p.DistrictId == districtId)
                        && (gradeId == 0 || p.GradeId == gradeId)
                        && (subjectId == 0 || p.SubjectId == subjectId)
                        );

            result.TotalElement = await queries.CountAsync();
            result.list = await queries.Skip(page * AppConfig.ROWS_ACCOUNT_LIST)
                        .Take(AppConfig.ROWS_ACCOUNT_LIST).Select(p => p.TutorCard).ToListAsync()
                            ;


            return result;
        }

        public async Task<PageTutorRequestListViewModel> GetTutorRequestCardByStatus(int districtId, int subjectId, int gradeId, int statusId, int page, int tutorId)
        {
            var result = new PageTutorRequestListViewModel();
            var queries = _context.RequestTutorForms
                .AsNoTracking()
                .Where(p => !p.TutorApplyForms.Any(taf => taf.TutorId == tutorId))
                .Select(p => new
                {
                    TutorCard = new TutorRequestCardViewModel
                    {
                        RequestId = p.Id,
                        GradeName = p.Grade.Name,
                        SubjectName = p.Subject.Name,
                        AdditionalDetail = p.AdditionalDetail ?? string.Empty,
                        Address = $"{p.District.Name}, {p.District.Province.Name}",
                        SessionsCanTeach = string.Join(", ", p.Sessions.Select(p => p.Name)),
                        Price = p.Grade.Fee,
                    },
                    GradeId = p.GradeId,
                    SubjectId = p.SubjectId,
                    DistrictId = p.DistrictId,
                    Status = p.StatusId,
                    Expired = p.ExpiredDate
                })
                .OrderBy(p => p.Expired)
                .Where(p => p.Status == statusId && p.Expired > DateTime.Now
                        && (districtId == 0 || p.DistrictId == districtId)
                        && (gradeId == 0 || p.GradeId == gradeId)
                        && (subjectId == 0 || p.SubjectId == subjectId)
                        )
                
                ;

            result.TotalElement = await queries.CountAsync();
            result.list = await queries.Skip(page * AppConfig.ROWS_ACCOUNT_LIST)
                        .Take(AppConfig.ROWS_ACCOUNT_LIST).Select(p => p.TutorCard).ToListAsync();
            return result;
        }

        public async Task<TutorRequestCardViewModel?> GetTutorRequestCardById(int requestId)
        {
            var result = await _context.RequestTutorForms
                .AsNoTracking()
                .Select(p => new TutorRequestCardViewModel
                {
                    RequestId = p.Id,
                    GradeName = p.Grade.Name,
                    SubjectName = p.Subject.Name,
                    AdditionalDetail = p.AdditionalDetail ?? string.Empty,
                    Address = $"{p.District.Name}, {p.District.Province.Name}",
                    AddressDetail = p.AddressDetail ?? string.Empty,
                    SessionsCanTeach = string.Join(", ", p.Sessions.Select(p => p.Name)),
                    Price = p.Grade.Fee,
                    RequestStatus = p.Status.Name,
                })
                .FirstOrDefaultAsync(p => p.RequestId == requestId);


            return result;
        }

        public async Task<TutorRequestProfileEditViewModel?> GetTutorRequestProfileEdit(int id)
        {
            return await _context.RequestTutorForms
                .Select(p => new TutorRequestProfileEditViewModel
                {

                    DistrictId = p.DistrictId,
                    AdditionalDetail = p.AdditionalDetail,
                    Addressdetail = p.AddressDetail,
                    CreateDate = p.CreateDate,
                    ExpireDate = p.ExpiredDate,
                    CurrentStatus = p.Status.Name,
                    FullName = p.Customer.FullName,
                    GradeId = p.GradeId,
                    NStudents = p.Students,
                    ProvinceId = p.District.ProvinceId,
                    SelectedBefore = p.Sessions.Select(p => p.Id).ToList(),
                    RequestId = p.Id,
                    SubjectId = p.SubjectId,
                })
                .FirstOrDefaultAsync(p => p.RequestId == id);
        }

        public async Task<List<CustomerTutorRequestViewModel>> GetListTutorRequestOfCustomer(int customerId)
        {
            return await _context.RequestTutorForms
                .AsNoTracking()
                .Select(p => new {
                    CustomerRequest = new CustomerTutorRequestViewModel
                    {
                        AdditionalDetail = p.AdditionalDetail,
                        AddressDetail = $"{p.District.Province}, {p.District.Name}, {p.AddressDetail}",
                        CreateDate = p.CreateDate,
                        ExpiredDate = p.ExpiredDate,
                        GradeName = p.Grade.Name,
                        RequestId = p.Id,
                        StatusName = p.Status.Name,
                        Students = p.Students,
                        SubjectName = p.Subject.Name,
                    },
                    p.CustomerId,
                })
                .OrderByDescending(p => p.CustomerRequest.CreateDate)
                .Where(p => p.CustomerId == customerId)
                .Select(p => p.CustomerRequest)
                .ToListAsync();
        }

        public async Task<bool> UpdateTutorRequestProfileEdit(TutorRequestProfileEditViewModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var profileEdit = await _context.RequestTutorForms
                        .Include(r => r.Sessions)
                        .FirstOrDefaultAsync(r => r.Id == model.RequestId);

                    if (profileEdit == null) { return false; }

                    // Remove sessions that are not in the list of selectedSessionDateIds
                    var sessionsToRemove = profileEdit.Sessions
                        .Where(s => !model.SessionSelected.Contains(s.Id))
                        .ToList();

                    foreach (var session in sessionsToRemove)
                    {
                        profileEdit.Sessions.Remove(session);
                    }

                    // Add new sessions that are not already in the existing sessions
                    var newSessionIds = model.SessionSelected
                        .Where(id => !profileEdit.Sessions.Any(s => s.Id == id))
                        .ToList();

                    foreach (var sessionId in newSessionIds)
                    {
                        var sessionToAdd = _context.SessionDates.Find(sessionId);
                        if (sessionToAdd != null)
                        {
                            profileEdit.Sessions.Add(sessionToAdd);
                        }
                    }

                    // Modify attribute not many-to-many
                    profileEdit.Students = model.NStudents;
                    profileEdit.AdditionalDetail = model.AdditionalDetail;
                    profileEdit.AddressDetail = model.Addressdetail;
                    profileEdit.DistrictId = model.DistrictId;
                    profileEdit.SubjectId = model.SubjectId;
                    profileEdit.GradeId = model.GradeId;

                    _context.RequestTutorForms.Update(profileEdit);

                    int result = _context.SaveChanges();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return true;
                    }
                }
                catch(Exception)
                {
                    transaction.Rollback();
                }
            }

            return false;
        }

        public async Task<bool> UpdateTutorApplyStatus(int tutorId, int requestId, Status newStatus)
        {
            var tutorApply = await _context.TutorApplyForms
                .FirstOrDefaultAsync(p => p.TutorId == tutorId && p.TutorRequestId == requestId);

            if (tutorApply == null) return false;

            tutorApply.Status = newStatus;
            _context.TutorApplyForms.Update(tutorApply);

            return _context.SaveChanges() > 0;
        }
    }
}
