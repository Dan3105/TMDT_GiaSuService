using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Repository
{
    public class TutorRepo : ITutorRepo
    {
        private readonly DvgsDbContext _context;
        public TutorRepo(DvgsDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<AccountListViewModel>> GetTutorAccountsByFilter(
            int provinceId, int districtId, int subjectId, int gradeId, int page)
        {
            var filteredTutors = _context.Tutors
                .AsNoTracking()
                .Select(p => new
                {
                    Tutors = new AccountListViewModel()
                    {
                        Email = p.Account.Email,
                        FullName = p.FullName,
                        LockStatus = p.Account.LockEnable,
                        ImageUrl = p.Account.Avatar,
                        CreateDate = p.Account.CreateDate.ToString("dd/MM/yyyy")
                    },
                    CreateDate = p.Account.CreateDate,
                    StatusName = p.Status.Name,
                    Subjects = p.Subjects.Select(s => s.Id),
                    Districts = p.Districts.Select(d => d.Id),
                    Provinces = p.Districts.Select(d => d.Province.Id),
                    Grades = p.Grades.Select(g => g.Id)
                }).OrderByDescending(p => p.CreateDate)
                .Where(p => (subjectId == 0 || p.Subjects.Contains(subjectId))
                        && (provinceId == 0 || p.Provinces.Contains(provinceId))
                        && (districtId == 0 || p.Districts.Contains(districtId))
                        && (gradeId == 0 || p.Grades.Contains(gradeId))
                        && (p.StatusName == RegisterStatus.APPROVAL.ToString().ToLower()))
                ;

            filteredTutors = filteredTutors.Skip(page * ROWS_ACCOUNT_LIST)
                .Take(ROWS_ACCOUNT_LIST)
                ;

            return await filteredTutors.Select(p => p.Tutors).ToListAsync();
        }


        public async Task<PageCardListViewModel> GetTutorCardsByFilter(
            int provinceId, int districtId, int subjectId, int gradeId, int page)
        {
            var query = _context.Tutors
                .AsNoTracking()
                .Select(p => new
                {
                    tutor = new TutorCardViewModel()
                    {
                        Id = p.Id,
                        AdditionalProfile = p.AdditionalInfo ?? "",
                        Area = p.Area,
                        Avatar = p.Account.Avatar,
                        Birth = p.Birth.ToString("dd/MM/yyyy"),
                        College = p.College,
                        TutorType = p.TutorType.Name,
                        FullName = p.FullName,
                        GraduateYear = p.AcademicYearTo,
                        GradeList = string.Join(", ", p.Grades.Select(g => g.Name)),
                        TeachingArea = string.Join(", ", p.Districts.Select(d => d.Name)),
                        SubjectList = string.Join(", ", p.Subjects.Select(g => g.Name)),
                        IsActive = p.IsActive ?? false,
                        IsEnable = !p.Account.LockEnable,
                        CreateDate = p.Account.CreateDate.ToString("dd/MM/yyyy")
                    },
                    CreateDate = p.Account.CreateDate,
                    StatusName = p.Status.Name,
                    Subjects = p.Subjects.Select(s => s.Id),
                    Districts = p.Districts.Select(d => d.Id),
                    Provinces = p.Districts.Select(d => d.Province.Id),
                    Grades = p.Grades.Select(g => g.Id)
                }).OrderByDescending(p => p.CreateDate)
                .Where(p => (subjectId == 0 || p.Subjects.Contains(subjectId))
                            && (provinceId == 0 || p.Provinces.Contains(provinceId))
                            && (districtId == 0 || p.Districts.Contains(districtId))
                            && (gradeId == 0 || p.Grades.Contains(gradeId))
                            && (p.StatusName == RegisterStatus.APPROVAL.ToString().ToLower()));
            
            var result = new PageCardListViewModel();
            result.TotalElement = await query.CountAsync();

            query = query.Skip(page * ROWS_ACCOUNT_LIST)
                            .Take(ROWS_ACCOUNT_LIST);

            result.list = await query.Select(p => p.tutor).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Tutor>> GetTutorprofilesByClassId(int classId)
        {
            //Get list tutorId by classId
            var tutorIds = await _context.TutorApplyForms
                                .Where(p => p.TutorRequestId == classId)
                                .Include(p => p.Tutor)
                                .Select(p => p.TutorId)
                                .ToListAsync();

            // Get list Tutorprofile by tutorId
            var tutorProfiles = await _context.Tutors
                .Where(p => tutorIds.Contains(p.Id))
                .ToListAsync();

            return tutorProfiles;
        }

        public async Task<List<TutorCardViewModel>> GetSubTutorCardView(List<int> ids)
        {
            var tutors = await _context.Tutors
                .Select(p => new TutorCardViewModel
                {
                    Id = p.Id,
                    Avatar = p.Account.Avatar,
                    FullName = p.FullName,
                    Area = p.Area,
                    College = p.College,
                    TutorType = p.TutorType.Name,
                    IsActive= p.IsActive ?? false,
                })
                .Where(p => ids.Contains(p.Id)).ToListAsync();
            return tutors;
        }

        public async Task<PageTutorRegisterListViewModel> GetRegisterTutorOnPending(int page, RegisterStatus status)
        {
            var query = _context.Tutors.AsNoTracking()
                .Select(p => new
                {
                    View = new TutorRegisterViewModel
                    {
                        Id = p.Id,
                        Area = p.Area,
                        College = p.College,
                        CreateDate = DateOnly.FromDateTime(p.Account.CreateDate),
                        CurrentStatus = p.TutorType.Name,
                        FullName = p.FullName,
                        StatusQuery = p.Status.VietnameseName
                    },
                    StatusName = p.Status.Name
                })
                .Where(p => p.StatusName.ToLower() == status.ToString().ToLower())
                .Select(p => p.View);


            var result = new PageTutorRegisterListViewModel { };
            result.TotalElement = await query.CountAsync();
            result.list = await query.OrderByDescending(p => p.CreateDate)
                    .Skip(page * ROWS_ACCOUNT_LIST)
                    .Take(ROWS_ACCOUNT_LIST)
                    .ToListAsync();
                        
            return result;
        }

        public async Task<Tutor?> GetTutor(int id)
        {
            return await _context.Tutors
                .Include(p => p.Identity)
                .Include(p => p.Account)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> UpdateTutor(Tutor tutor)
        {
            try
            {
                _context.Tutors.Update(tutor);
                return await SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<TutorProfileStatusDetailHistoryViewModel?> GetATutorProfileHistoryDetail(int statusDetail)
        {
            return await _context.TutorStatusDetails
                .Select(p => new TutorProfileStatusDetailHistoryViewModel
                {
                    Date = p.CreateDate,
                    HistoryId = p.Id,
                    Context = p.Context,
                    StatusType = p.Status.Name
                })
                .FirstOrDefaultAsync(p => p.HistoryId == statusDetail);
        }

        public async Task<IEnumerable<TutorProfileStatusDetailHistoryViewModel>> GetTutorProfilesHistoryDetail(int tutorId)
        {
            return await _context.TutorStatusDetails.Where(p => p.TutorId == tutorId)
                .OrderByDescending(p => p.CreateDate)
                .Select(p => new TutorProfileStatusDetailHistoryViewModel   
                {
                    HistoryId = p.Id,
                    StatusType = p.Status.Name,
                    Context = p.Context,
                    Date = p.CreateDate,
                    StatusVNamese = p.Status.VietnameseName,

                }).ToListAsync();
        }

        public async Task<TutorStatusDetail?> GetLatestTutorStatusDetail(int tutorId)
        {
            var getLatestStatus = await _context.TutorStatusDetails
                         .Include(p => p.Status)
                         .AsNoTracking()
                         .OrderByDescending(p => p.CreateDate)
                         .FirstOrDefaultAsync(p => p.TutorId == tutorId);
            return getLatestStatus;
        }

        public async Task<List<TutorApplyCardViewModel>> GetListTutorApplyForm(int tutorId)
        {
            return await _context.TutorApplyForms
                .AsNoTracking()
                .Select(p => new {
                    TutorApply = new TutorApplyCardViewModel
                    {
                        RequestId = p.TutorRequest.Id,

                        EnterDate = p.EnterDate,
                        ExpiredDate = p.TutorRequest.ExpiredDate,

                        SubjectName = p.TutorRequest.Subject.Name,
                        GradeName = p.TutorRequest.Grade.Name,
                        NStudents = p.TutorRequest.Students,
                        Location = $"{p.TutorRequest.District.Name}, {p.TutorRequest.District.Province.Name}",

                        RequestStatus = p.TutorRequest.Status.Name,
                        RequestStatusDescription = p.TutorRequest.Status.VietnameseName,
                        QueueStatus = p.Status.Name,
                        QueueStatusDescription = p.Status.VietnameseName,
                    },
                    p.TutorId,
                })
                .OrderByDescending(p => p.TutorApply.EnterDate)
                .Where(p => p.TutorId == tutorId)
                .Select(p => p.TutorApply)
                .ToListAsync();
        }

        public async Task<RequestTutorApplyDetailViewModel?> GetRequestTutorApplyDetail(int requestId, int tutorId)
        {
            var result = await _context.TutorApplyForms
                .AsNoTracking()
                .Select(p => new RequestTutorApplyDetailViewModel
                {
                    RequestId = p.TutorRequest.Id,
                    TutorId = p.TutorId,

                    NStudent = p.TutorRequest.Students,
                    CustomerFullName = p.TutorRequest.Customer.FullName,
                    CustomerEmail = p.TutorRequest.Customer.Account.Email,
                    CustomerPhone = p.TutorRequest.Customer.Account.Phone,
                    GradeName = p.TutorRequest.Grade.Name,
                    SubjectName = p.TutorRequest.Subject.Name,

                    Address = $"{p.TutorRequest.AddressDetail}",
                    Location = $"{p.TutorRequest.District.Name}, {p.TutorRequest.District.Province.Name}",
                    AdditionalDetail = $"{p.TutorRequest.AdditionalDetail}",
                    
                    Price = p.TutorRequest.Grade.Fee,

                    RequestStatus = p.TutorRequest.Status.Name,
                    RequestStatusDescription = p.TutorRequest.Status.VietnameseName,
                    QueueStatus = p.Status.Name,
                    QueueStatusDescription = p.Status.VietnameseName
                }).FirstOrDefaultAsync(p => p.RequestId == requestId && p.TutorId == tutorId);
            return result;
        }

    }
}
