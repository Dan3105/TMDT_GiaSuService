using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TutormatchrequestqueueService : ITutormatchrequestqueueService
    {
        private readonly ITutormatchrequestqueueRepository _repo;
        public TutormatchrequestqueueService(ITutormatchrequestqueueRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> CreateRequest(Tutormatchrequestqueue req)
        {
            try
            {
                _repo.Create(req);
                var isSucced = await _repo.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateRequest(int classId, int tutorId, AppConfig.QueueStatus status)
        {
            try
            {
                var isSucced = await _repo.UpdateTutorRequestQueueStatus(classId, tutorId, status);
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
