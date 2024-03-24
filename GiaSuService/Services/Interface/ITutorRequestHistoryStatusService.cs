using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorRequestHistoryStatusService
    {
        public Task<List<Tutorrequesthistorystatus>> GetByTutorRequestFormId(int reqFormId);
        public Task<bool> CreateRequestHistory(Tutorrequesthistorystatus form);
        public Task<bool> UpdateRequestHistory(Tutorrequesthistorystatus form);
    }
}
