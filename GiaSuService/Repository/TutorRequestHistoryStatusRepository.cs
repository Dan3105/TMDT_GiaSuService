using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class TutorRequestHistoryStatusRepository : ITutorRequestHistoryStatusRepository
    {
        private readonly TmdtDvgsContext _context;

        public TutorRequestHistoryStatusRepository(TmdtDvgsContext context)
        {
            _context = context;
        }

        public void Create(Tutorrequesthistorystatus entity)
        {
            _context.Tutorrequesthistorystatuses.Add(entity);
        }

        public void Update(Tutorrequesthistorystatus entity)
        {
            _context.Tutorrequesthistorystatuses.Update(entity);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Delete(Tutorrequesthistorystatus entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tutorrequesthistorystatus>> GetByTutorFormRequestId(int formId)
        {
            return (await _context.Tutorrequesthistorystatuses
                .Where(p => p.Tutorrequestformid == formId)
                .ToListAsync());
        }

        
    }
}
