using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
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
            int subjectId, int districtId, int gradeId, int page)
        {
            var filteredTutors = _context.Tutors
                .AsNoTracking()
                .Select(p => new
                {
                    Tutors = new AccountListViewModel()
                    {
                        Email = p.Account.Email,
                        FullName = p.Fullname,
                        LockStatus = p.Account.Lockenable ?? true,
                        ImageUrl = p.Account.Avatar,
                    },
                    CreateDate = p.Account.Createdate,
                    IsValid = p.Isvalid,
                    Subjects = p.Subjects.Select(s => s.Id),
                    Districts = p.Districts.Select(d => d.Id),
                    Grades = p.Grades.Select(g => g.Id)
                }).OrderByDescending(p => p.CreateDate)
                .Where(p => (subjectId == 0 || p.Subjects.Contains(subjectId))
                        && (districtId == 0 || p.Districts.Contains(districtId))
                        && (gradeId == 0 || p.Grades.Contains(gradeId))
                        && p.IsValid)
                ;

            filteredTutors = filteredTutors.Skip(page * AppConfig.ROWS_ACCOUNT_LIST)
                .Take(AppConfig.ROWS_ACCOUNT_LIST)
                ;

            return await filteredTutors.Select(p => p.Tutors).ToListAsync();
        }


        public async Task<IEnumerable<TutorCardViewModel>> GetTutorCardsByFilter(int subjectId, int districtId, int gradeId, int page)
        {
            var query = _context.Tutors
                .AsNoTracking()
                .Where(tutor => (subjectId == 0 || tutor.Subjects.Any(s => s.Id == subjectId))
                               && (districtId == 0 || tutor.Districts.Any(d => d.Id == districtId))
                               && (gradeId == 0 || tutor.Grades.Any(g => g.Id == gradeId))
                               && tutor.Isvalid)
                .OrderByDescending(p => p.Account.Createdate)
                .Select(tutor => new TutorCardViewModel
                {
                    Id = tutor.Id,
                    AdditionalProfile = tutor.Additionalinfo ?? "",
                    Area = tutor.Area,
                    Avatar = tutor.Account.Avatar,
                    Birth = ((DateOnly)tutor.Birth!).ToString("dd/MM/yyyy"),
                    College = tutor.College,
                    TutorType = tutor.Typetutor ? "Giáo viên" : "Sinh viên",
                    FullName = tutor.Fullname,
                    GraduateYear = tutor.Academicyearto,
                    GradeList = string.Join(", ", tutor.Grades.Select(g => g.Name)),
                    TeachingArea = string.Join(", ", tutor.Districts.Select(d => d.Name)),
                    SubjectList = string.Join(", ", tutor.Subjects.Select(g => g.Name)),
                })
                .Skip(page * AppConfig.ROWS_ACCOUNT_LIST)
                .Take(AppConfig.ROWS_ACCOUNT_LIST);

            return await query.ToListAsync();
        }


        public async Task<IEnumerable<Tutor>> GetTutorprofilesByClassId(int classId)
        {
            //Get list tutorId by classId
            var tutorIds = await _context.Tutorqueues
                                .Where(p => p.Tutorrequestid == classId)
                                .Include(p => p.Tutor)
                                .Select(p => p.Tutorid)
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
                    FullName = p.Fullname,
                    Area = p.Area,
                    College = p.College,
                    TutorType = p.Typetutor ? "Giáo viên" : "Sinh viên",
                })
                .Where(p => ids.Contains(p.Id)).ToListAsync();
            return tutors;
        }

        public async Task<List<TutorRegisterViewModel>> GetRegisterTutorOnPending(int page)
        {
            var tutor_queues = _context.Tutors.AsNoTracking()
                .Select(p => new TutorRegisterViewModel
                {
                    Id = p.Id,
                    Area = p.Area,
                    College = p.College,
                    CreateDate = DateOnly.FromDateTime((DateTime)p.Account.Createdate!),
                    CurrentStatus = p.Typetutor ? "Gia sư" : "Sinh viên",
                    FullName = p.Fullname,
                    IsValid = p.Isvalid,
                    StatusQuery = "Đang chờ duyệt"
                })
                .OrderByDescending(p => p.CreateDate)
                .Where(p => p.IsValid == false);


            tutor_queues = tutor_queues.Skip(page * AppConfig.ROWS_ACCOUNT_LIST).Take(AppConfig.ROWS_ACCOUNT_LIST);
            return await tutor_queues.ToListAsync(); ;
        }

        public async Task<Tutor?> GetTutor(int id)
        {
            return await _context.Tutors.FindAsync(id);
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

    }
}
