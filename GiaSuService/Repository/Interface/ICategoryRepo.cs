using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface ICategoryRepo
    {
        public Task<TutorTypeViewModel> GetTutorTypeById(int typeId);
        public Task<List<TutorTypeViewModel>> GetAllTutorTypes();

        public Task<GradeViewModel> GetGradeById(int gradeId);
        public Task<List<GradeViewModel>> GetAllGrades();
        public Task<List<GradeViewModel>> GetSubGrades(List<int> ids);

        public Task<bool> CreateSubject(SubjectViewModel subject);
        public Task<bool> UpdateSubject(SubjectViewModel subject);
        public Task<bool> DeleteSubject(int id);
        public Task<SubjectViewModel> GetSubjectById(int subjectId);
        public Task<List<SubjectViewModel>> GetAllSubjects();
        public Task<List<SubjectViewModel>> GetSubSubjects(List<int> ids);

        public Task<bool> CreateSessionDate(SessionViewModel session);
        public Task<bool> UpdateSessionDate(SessionViewModel session);
        public Task<bool> DeleteSessionDate(int id);
        public Task<SessionViewModel?> GetSessionById(int sessionId);
        public Task<List<SessionViewModel>> GetAllSessions();
        public Task<List<SessionViewModel>> GetSubSessions(List<int> ids);

        public Task<bool> IsUniqueName(string name, Type type);
    }
}
