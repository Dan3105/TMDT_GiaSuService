using GiaSuService.AppDbContext;
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
                .Where(p => p.Statustype.Type == typeStatus)
                .Select(p => p.Name).ToListAsync();
        }

        public async Task<int?> GetStatusId(string nameStatus, string typeStatus)
        {
            return (await _context.Statuses.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Statustype.Type == typeStatus && p.Name == nameStatus)
                )?.Id;
        }
    }
}
