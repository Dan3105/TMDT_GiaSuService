using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class TutormatchrequestqueueRepository : ITutormatchrequestqueueRepository
    {
        private readonly TmdtDvgsContext _context;

        public TutormatchrequestqueueRepository(TmdtDvgsContext context)
        {
            _context = context;
        }

        public void Create(Tutormatchrequestqueue entity)
        {
            _context.Tutormatchrequestqueues.Add(entity);
        }

        public void Update(Tutormatchrequestqueue entity)
        {
            _context.Tutormatchrequestqueues.Update(entity);
        }

        public void Delete(Tutormatchrequestqueue entity)
        {
            _context.Tutormatchrequestqueues.Remove(entity);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> UpdateTutorRequestQueueStatus(int classId, int tutorId, AppConfig.QueueStatus status)
        {
            Tutormatchrequestqueue? req = await _context.Tutormatchrequestqueues
                                    .Where(p => p.Formid == classId
                                    && p.Tutorid == tutorId)
                                    .FirstOrDefaultAsync();
            if (req != null)
            {
                req.Queuestatus = status;
                _context.Tutormatchrequestqueues.Update(req);
                return await SaveChanges();
            }

            return false;
        }
    }
}
