using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorRequestHistoryStatusService
    {
        public Task<List<Requeststatus>> GetByTutorRequestFormId(int reqFormId);
        public Task<bool> CreateRequestHistory(Requeststatus form);
        public Task<bool> UpdateRequestHistory(Requeststatus form);
    }
}
