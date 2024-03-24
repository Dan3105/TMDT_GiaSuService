using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutormatchrequestqueueService
    {
        public Task<bool> CreateRequest(Tutormatchrequestqueue req);
        public Task<bool> UpdateRequest(int classId, int tutorId, AppConfig.QueueStatus status);
    }
}
