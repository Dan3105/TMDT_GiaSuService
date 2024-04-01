using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly DvgsDbContext _context;
        public CategoryRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public async Task<List<GradeViewModel>> GetAllGrades()
        {
            return await _context.Grades.AsNoTracking()
                .Select(p => new GradeViewModel
                {
                    GradeId = p.Id,
                    GradeName = p.Name,
                })
                .ToListAsync();
        }

        public async Task<List<SessionViewModel>> GetAllSessions()
        {
            return await _context.Sessiondates.AsNoTracking()
                .Select(p => new SessionViewModel
                {
                    SessionId = p.Id,
                    SessionName = p.Name
                })
                .ToListAsync();
        }

        public async Task<List<SubjectViewModel>> GetAllSubjects()
        {
            return await _context.Subjects.AsNoTracking()
                .Select(p=> new SubjectViewModel
                {
                    SubjectId = p.Id,
                    SubjectName = p.Name
                })
                .ToListAsync();
        }

        public Task<GradeViewModel> GetGradeById(int gradeId)
        {
            throw new NotImplementedException();
        }

        public Task<SessionViewModel> GetSessionById(int sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<GradeViewModel>> GetSubGrades(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<SubjectViewModel> GetSubjectById(int subjectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SessionViewModel>> GetSubSessions(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<SubjectViewModel>> GetSubSubjects(List<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
