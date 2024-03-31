using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRequestHistoryStatusRepository : IRepository<Requeststatus>
    {
        public Task<IEnumerable<Requeststatus>> GetByTutorRequestFormId(int formId);
    }
}
