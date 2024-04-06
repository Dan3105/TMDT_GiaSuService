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

        public void Create(RequestStatus entity)
        {
            _context.RequestStatuses.Add(entity);
        }

        public void Update(RequestStatus entity)
        {
            _context.RequestStatuses.Update(entity);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Delete(RequestStatus entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RequestStatus>> GetByTutorRequestFormId(int formId)
        {
            return (await _context.RequestStatuses
                .Where(p => p.Id == formId)
                .ToListAsync());
        }

        
    }
}
