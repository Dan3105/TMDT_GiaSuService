using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ISessionRepository : IRepository<Sessiondate>
    {
        Task<Sessiondate> GetSessionById(int sessionId);
        Task<List<Sessiondate>> GetAllSessions();
    }
}
