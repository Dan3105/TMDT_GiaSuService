using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class QueueRepo : IQueueRepo
    {
        private readonly DvgsDbContext _context;
        public QueueRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public bool AddTutorsToQueue(Tutorrequestform form, List<int> ids, int statusDefaultId)
        {
            try {
                form.Tutorqueues = new List<Tutorqueue>();
                foreach(var id in ids)
                {
                    form.Tutorqueues.Add(
                    new Tutorqueue
                    {
                        Enterdate = DateOnly.FromDateTime(DateTime.Now),
                        Statusid = statusDefaultId,
                        Tutorid = id,
                    });
                }

                _context.Update(form);
                return true;
            }
            catch (Exception) {
                return false;
            }
           
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
