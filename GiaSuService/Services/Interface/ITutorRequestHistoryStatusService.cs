using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorRequestHistoryStatusService
    {
        public Task<List<RequestStatus>> GetByTutorRequestFormId(int reqFormId);
        public Task<bool> CreateRequestHistory(RequestStatus form);
        public Task<bool> UpdateRequestHistory(RequestStatus form);
    }
}
