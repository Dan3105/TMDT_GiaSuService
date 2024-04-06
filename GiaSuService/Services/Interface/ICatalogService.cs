using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ICatalogService
    {
        Task<List<TutorTypeViewModel>> GetAllTutorType();
        Task<TutorTypeViewModel> GetTutorTypeById(int tutorTypeId);

        // CRUD operations for Sessions
        Task<List<SessionViewModel>> GetAllSessions();
        Task<SessionViewModel> GetSessionById(int sessionId);

        // CRUD operations for Grades
        Task<List<GradeViewModel>> GetAllGrades();
        Task<GradeViewModel> GetGradeById(int gradeId);

        // CRUD operations for Subjects
        Task<List<SubjectViewModel>> GetAllSubjects();
        Task<SubjectViewModel> GetSubjectById(int subjectId);
    }
}
