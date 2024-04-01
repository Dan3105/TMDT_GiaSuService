using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface ICategoryRepo
    {
        public Task<GradeViewModel> GetGradeById(int gradeId);
        public Task<List<GradeViewModel>> GetAllGrades();
        public Task<List<GradeViewModel>> GetSubGrades(List<int> ids);

        public Task<SubjectViewModel> GetSubjectById(int subjectId);
        public Task<List<SubjectViewModel>> GetAllSubjects();
        public Task<List<SubjectViewModel>> GetSubSubjects(List<int> ids);

        public Task<SessionViewModel> GetSessionById(int sessionId);
        public Task<List<SessionViewModel>> GetAllSessions();
        public Task<List<SessionViewModel>> GetSubSessions(List<int> ids);

    }
}
