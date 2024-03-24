using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutormatchrequestqueueRepository : IRepository<Tutormatchrequestqueue>
    {
        public Task<bool> UpdateTutorRequestQueueStatus(int classId, int tutorId, AppConfig.QueueStatus status);
    }
}
