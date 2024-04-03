using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRequestHistoryStatusRepo : IRepository<Requeststatus>
    {
        public Task<IEnumerable<Requeststatus>> GetByTutorRequestFormId(int formId);
    }
}
