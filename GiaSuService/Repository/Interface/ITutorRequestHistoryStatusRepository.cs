using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRequestHistoryStatusRepository : IRepository<Tutorrequesthistorystatus>
    {
        public Task<IEnumerable<Tutorrequesthistorystatus>> GetByTutorFormRequestId(int formId);
    }
}
