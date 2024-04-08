using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ICatalogService
    {
        Task<List<TutorTypeViewModel>> GetAllTutorType();
        Task<TutorTypeViewModel> GetTutorTypeById(int tutorTypeId);

        // CRUD operations for Sessions
        Task<ResponseService> UpdateSessionDate(SessionViewModel vm);
        Task<ResponseService> CreateSessionDate(SessionViewModel vm);
        Task<ResponseService> DeleteSessionDate(int id);
        Task<List<SessionViewModel>> GetAllSessions();

        // CRUD operations for Grades
        Task<List<GradeViewModel>> GetAllGrades();
        Task<GradeViewModel> GetGradeById(int gradeId);

        // CRUD operations for Subjects
        Task<ResponseService> UpdateSubject(SubjectViewModel vm);
        Task<ResponseService> CreateSubject(SubjectViewModel vm);
        Task<ResponseService> DeleteSubject(int id);
        Task<List<SubjectViewModel>> GetAllSubjects();
        Task<SubjectViewModel> GetSubjectById(int subjectId);
    }
}
