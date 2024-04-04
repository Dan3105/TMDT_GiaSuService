using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class TutorRequestRepo : ITutorRequestRepo
    {
        private readonly DvgsDbContext _context;

        public TutorRequestRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public void Create(Tutorrequestform entity)
        {
            _context.Tutorrequestforms.Add(entity);
        }

        public void Update(Tutorrequestform entity)
        {
            _context.Tutorrequestforms.Update(entity);
        }

        public void Delete(Tutorrequestform entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<Tutorrequestform?> Get(int id)
        {
            return (await _context.Tutorrequestforms
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.Id == id));
        }

        public async Task<List<Tutorrequestform>> GetAll()
        {
            return (await _context.Tutorrequestforms.ToListAsync());
        }

        public async Task<List<Tutorrequestform>> GetByFilter(int subjectId, int gradeId, int districtId)
        {
            var filteredForms = await _context.Tutorrequestforms
                .Where(p => (subjectId == 0 || p.Subjectid == subjectId)
                      && (gradeId == 0 || p.Gradeid == gradeId)
                      && (districtId == 0 || p.Districtid == districtId))
            .ToListAsync();

            return filteredForms;
        }

        public async Task<List<TutorRequestQueueViewModel>> GetTutorRequestQueueByStatus(int statusId, int page)
        {
            return await _context.Tutorrequestforms
                .AsNoTracking()
                .Select(p => new
                {
                    TutorRequest = new TutorRequestQueueViewModel
                    {
                        AddressName = $"{p.District.Province.Name}, {p.District.Name}, {p.Additionaldetail}",
                        CreatedDate = ((DateTime)p.Createddate!).ToString("dd-MM-yyyy HH:mm:ss"),
                        FormId = p.Id,
                        FullNameRequester = p.Customer.Fullname,
                        GradeName = p.Grade.Name,
                        SubjectName = p.Subject.Name,
                    },
                    ExpiredDate = p.Expireddate,
                    CreatedDate = p.Createddate,
                    StatusId = p.Statusid
                })
                .Where(p => p.ExpiredDate > DateTime.Now && p.StatusId == statusId)
                .OrderByDescending(p => p.CreatedDate)
                .Skip(page * AppConfig.ROWS_ACCOUNT_LIST)
                .Take(AppConfig.ROWS_ACCOUNT_LIST)
                .Select(p => p.TutorRequest)
                .ToListAsync();
        }

        public async Task<TutorRequestProfileViewModel?> GetTutorRequestProfile(int id)
        {
            var result = await _context.Tutorrequestforms
                .AsNoTracking()
                .Select(p => new TutorRequestProfileViewModel
                {
                    FormId = p.Id,
                    FullNameRequester = p.Customer.Fullname,
                    AddressName = $"{p.District.Province.Name}, {p.District.Name}, {p.Additionaldetail}",
                    CreatedDate = (DateTime)p.Createddate!,
                    CurrentStatus = p.Status.Name.ToString(),
                    ExpiredDate = (DateTime)p.Expireddate!,
                    GradeName = p.Grade.Name,
                    SubjectName = p.Subject.Name,

                }).FirstOrDefaultAsync(p => p.FormId == id);
            return result;
        }

        public async Task<List<TutorRequestCardViewModel>> GetTutorRequestCardByStatus(int districtId, int subjectId, int gradeId, int statusId, int page)
        {
            var result = _context.Tutorrequestforms
                .AsNoTracking()
                .Select(p => new
                {
                    TutorCard = new TutorRequestCardViewModel
                    {
                        RequestId = p.Id,
                        GradeName = p.Grade.Name,
                        SubjectName = p.Subject.Name,
                        AdditionalDetail = p.Additionaldetail ?? string.Empty,
                        Address = $"{p.District.Name}, {p.District.Name}, {p.Addressdetail}",
                        SessionsCanTeach = string.Join(", ", p.Sessions.Select(p => p.Name))
                    },
                    GradeId = p.Gradeid,
                    SubjectId = p.Subjectid,
                    DistrictId = p.Districtid,
                    Status = p.Statusid,
                    Expired = p.Expireddate
                })
                .OrderBy(p => p.Expired)
                .Where(p => p.Status == statusId && p.Expired > DateTime.Now 
                        && (districtId == 0 || p.DistrictId == districtId) 
                        && (gradeId == 0 || p.GradeId == gradeId) 
                        && (subjectId == 0 || p.SubjectId == subjectId)
                        )
                .Skip(page * AppConfig.ROWS_ACCOUNT_LIST)
                .Take(AppConfig.ROWS_ACCOUNT_LIST)
                ;
                

            return await result.Select(p => p.TutorCard).ToListAsync();
        }

        public async Task<TutorRequestProfileEditViewModel?> GetTutorRequestProfileEdit(int id)
        {
            return await _context.Tutorrequestforms
                .Select(p => new TutorRequestProfileEditViewModel
                {

                    DistrictId = p.Districtid,
                    AdditionalDetail = p.Additionaldetail,
                    Addressdetail = p.Addressdetail,
                    CreateDate = p.Createddate,
                    ExpireDate = p.Expireddate,
                    CurrentStatus = p.Status.Name,
                    FullName = p.Customer.Fullname,
                    GradeId = p.Gradeid,
                    NStudents = p.Students,
                    ProvinceId = p.District.Provinceid,
                    SelectedBefore = p.Sessions.Select(p => p.Id).ToList(),
                    RequestId = p.Id,
                    SubjectId = p.Subjectid,
                })
                .FirstOrDefaultAsync(p => p.RequestId == id);
        }
    }
}
