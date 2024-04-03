using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.TutorViewModel;
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
                        Enterdate = DateTime.Now,
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

        public async Task<List<TutorCardViewModel>> GetTutorInQueueByForm(int requestId, int statusId = 0)
        {
            var queries = await _context.Tutorqueues
                .Select(p => new
                {
                    Tutor = new TutorCardViewModel
                    {
                        Id = p.Tutorid,
                        Avatar = p.Tutor.Account.Avatar,
                        FullName = p.Tutor.Fullname,
                        Area = p.Tutor.Area,
                        College = p.Tutor.College,
                        TutorType = p.Tutor.Typetutor ? "Giáo viên" : "Sinh viên",
                    },
                    TutorRequestId = p.Tutorrequestid,
                    StatusId = p.Statusid
                })
                .Where(p => p.TutorRequestId == requestId && (statusId == 0 || statusId == p.StatusId))
                .Select(p => p.Tutor)
                .ToListAsync();
                ;

            return queries;
        }

    }
}
