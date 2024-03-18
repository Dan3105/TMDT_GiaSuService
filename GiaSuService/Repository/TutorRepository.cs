using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;

namespace GiaSuService.Repository
{
    public class TutorRepository : ITutorRepository
    {
        private readonly TmdtDvgsContext _context;
        public TutorRepository(TmdtDvgsContext context)
        {
            _context = context;
        }
        public void Create(Tutorprofile entity)
        {
            _context.Tutorprofiles.Add(entity);
        }

        public void Delete(Tutorprofile entity)
        {
            _context.Tutorprofiles.Remove(entity);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Update(Tutorprofile entity)
        {
            _context.Tutorprofiles.Update(entity);
        }
    }
}
