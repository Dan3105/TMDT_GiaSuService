using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface IQueueRepo
    {
        public bool AddTutorsToQueue(RequestTutorForm form, List<int> ids, int statusDefaultId);

        public Task<bool> AddTutorToQueue(int tutorId, int requestId, int statusId);

        public Task<bool> SaveChanges();

        public Task<List<TutorCardViewModel>> GetTutorInQueueByForm(int requestId, int statusId = 0);
    }
}
