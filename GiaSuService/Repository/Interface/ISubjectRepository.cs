using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject> GetSubjectById(int subjectId);
        Task<List<Subject>> GetAllSubjects();
    }
}
