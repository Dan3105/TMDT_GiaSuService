using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class TutorRequestFormRepository : ITutorRequestFormRepository
    {
        private readonly TmdtDvgsContext _context;

        public TutorRequestFormRepository(TmdtDvgsContext context)
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
                .Include(p => p.Account)
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

        public async Task<List<Tutorrequestform>> GetByStatus(AppConfig.TutorRequestStatus status)
        {
            return await _context.Tutorrequestforms
                .Include(p => p.Account)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.Createddate)
                .ToListAsync();
        }
    }
}
