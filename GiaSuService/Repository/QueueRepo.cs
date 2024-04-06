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

        public bool AddTutorsToQueue(RequestTutorForm form, List<int> ids, int statusDefaultId)
        {
            try {
                form.TutorApplyForms = new List<TutorApplyForm>();
                foreach(var id in ids)
                {
                    form.TutorApplyForms.Add(
                    new TutorApplyForm
                    {
                        EnterDate = DateTime.Now,
                        StatusId = statusDefaultId,
                        TutorId = id,
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
            var queries = await _context.TutorApplyForms
                .Select(p => new
                {
                    Tutor = new TutorCardViewModel
                    {
                        Id = p.TutorId,
                        Avatar = p.Tutor.Account.Avatar,
                        FullName = p.Tutor.FullName,
                        Area = p.Tutor.Area,
                        College = p.Tutor.College,
                        //TutorType = p.Tutor.TypeTutor,
                    },
                    TutorRequestId = p.TutorRequestId,
                    StatusId = p.StatusId
                })
                .Where(p => p.TutorRequestId == requestId && (statusId == 0 || statusId == p.StatusId))
                .Select(p => p.Tutor)
                .ToListAsync();
                ;

            return queries;
        }

    }
}
