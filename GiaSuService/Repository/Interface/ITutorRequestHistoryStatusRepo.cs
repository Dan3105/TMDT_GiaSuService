using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRequestHistoryStatusRepo : IRepository<RequestStatus>
    {
        public Task<IEnumerable<RequestStatus>> GetByTutorRequestFormId(int formId);
    }
}
