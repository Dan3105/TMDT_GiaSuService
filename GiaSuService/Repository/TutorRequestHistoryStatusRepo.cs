using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class TutorRequestHistoryStatusRepo : ITutorRequestHistoryStatusRepo
    {
        private readonly DvgsDbContext _context;

        public TutorRequestHistoryStatusRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public void Create(Requeststatus entity)
        {
            _context.Requeststatuses.Add(entity);
        }

        public void Update(Requeststatus entity)
        {
            _context.Requeststatuses.Update(entity);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Delete(Requeststatus entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Requeststatus>> GetByTutorRequestFormId(int formId)
        {
            return (await _context.Requeststatuses
                .Where(p => p.Id == formId)
                .ToListAsync());
        }

        
    }
}
