using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class StatusRepo : IStatusRepo
    {
        private readonly DvgsDbContext _context;
        public StatusRepo(DvgsDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetAllStatus(string typeStatus)
        {
            return await _context.Statuses.AsNoTracking()
                .Where(p => p.StatusType.Type == typeStatus)
                .Select(p => p.Name).ToListAsync();
        }

        public async Task<string?> GetLatestStatusInTutorRegister(int tutorId)
        {
            var tutorRequest = await _context.TutorStatusDetails.AsNoTracking()
                .Select(p => new { TutorId = p.TutorId, StatusName = p.Status.Name, ReviewingDate = p.CreateDate })
                .Where(p => p.TutorId == tutorId)
                .OrderByDescending(p => p.ReviewingDate)
                .FirstOrDefaultAsync();
            return tutorRequest?.StatusName;
        }

        public async Task<Status?> GetStatus(string nameStatus, string typeStatus)
        {
            return (await _context.Statuses
                .FirstOrDefaultAsync(p => p.StatusType.Type == typeStatus && p.Name.ToLower() == nameStatus.ToLower())
                );
        }

        public async Task<Tutor?> GetTutor(int id)
        {
            return await _context.Tutors.FindAsync(id);
        }

    }
}
