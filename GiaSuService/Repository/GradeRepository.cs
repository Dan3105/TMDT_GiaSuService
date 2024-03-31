using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class GradeRepository : IGradeRepository
    {
        private readonly DvgsDbContext _context;
        public GradeRepository(DvgsDbContext context)
        {
            _context = context;
        }

        public void Create(Grade entity)
        {
            _context.Grades.Add(entity);
        }

        public void Delete(Grade entity)
        {
            _context.Grades.Remove(entity);
        }

        public async Task<List<Grade>> GetAllGrades()
        {
            return await _context.Grades.OrderBy(p => p.Value).ToListAsync();
        }

        public async Task<Grade> GetGradeById(int gradeId)
        {
            return (await _context.Grades.FindAsync(gradeId))!;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Update(Grade entity)
        {

            _context.Grades.Update(entity);
        }
    }
}
