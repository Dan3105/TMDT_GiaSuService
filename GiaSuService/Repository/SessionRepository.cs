using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DvgsDbContext _context;
        public SessionRepository(DvgsDbContext context)
        {
            _context = context;
        }

        public void Create(Sessiondate entity)
        {
            _context.Sessiondates.Add(entity);

        }

        public void Delete(Sessiondate entity)
        {
            _context.Sessiondates.Remove(entity);
        }

        public async Task<List<Sessiondate>> GetAllSessions()
        {
            return await _context.Sessiondates.OrderBy(p => p.Value).ToListAsync();
        }

        public async Task<Sessiondate> GetSessionById(int sessionId)
        {
            return (await _context.Sessiondates.FindAsync(sessionId))!;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Update(Sessiondate entity)
        {
            _context.Sessiondates.Update(entity);

        }
    }
}
