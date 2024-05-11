using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
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
        public async Task<List<StatusNamePair>> GetAllStatus(string typeStatus)
        {
            return await _context.Statuses.AsNoTracking()
                .Where(p => p.StatusType.Type == typeStatus)
                .Select(p =>new StatusNamePair { Name = p.Name, VietnameseName=p.VietnameseName}).ToListAsync();
        }

        public async Task<string?> GetLatestStatusInTutorRegister(int tutorId, bool isTranslator)
        {
            var tutorRequest = await _context.Tutors.AsNoTracking()
                .Select(p => new { TutorId = p.Id, StatusName = p.Status.Name, VietnameseName = p.Status.VietnameseName })
                .FirstOrDefaultAsync(p => p.TutorId == tutorId);
            return isTranslator ? tutorRequest?.VietnameseName : tutorRequest?.StatusName;
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
