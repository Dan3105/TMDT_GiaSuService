using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly TmdtDvgsContext _context;
        public SubjectRepository(TmdtDvgsContext context)
        {
            _context = context;
        }

        public void Create(Subject entity)
        {
            _context.Subjects.Add(entity);
        }

        public void Delete(Subject entity)
        {
            _context.Subjects.Remove(entity);
        }

        public async Task<List<Subject>> GetAllSubjects()
        {
            return await _context.Subjects.OrderBy(p => p.Value).ToListAsync();
        }

        public async Task<Subject> GetSubjectById(int subjectId)
        {
            return (await _context.Subjects.FindAsync(subjectId))!;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Update(Subject entity)
        {

            _context.Subjects.Update(entity);

        }
    }
}
