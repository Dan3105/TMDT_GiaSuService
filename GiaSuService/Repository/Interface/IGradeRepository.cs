using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IGradeRepository : IRepository<Grade>
    {
        Task<Grade> GetGradeById(int gradeId);
        Task<List<Grade>> GetAllGrades();
    }
}
