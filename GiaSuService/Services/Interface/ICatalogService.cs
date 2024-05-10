using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ICatalogService
    {
        Task<List<StatusNamePair>> GetAllStatus(string statusType);
        Task<List<TutorTypeViewModel>> GetAllTutorType();

        // CRUD operations for Sessions
        Task<ResponseService> UpdateSessionDate(SessionViewModel vm);
        Task<ResponseService> CreateSessionDate(SessionViewModel vm);
        Task<ResponseService> DeleteSessionDate(int id);
        Task<List<SessionViewModel>> GetAllSessions();

        // CRUD operations for Grades
        Task<ResponseService> UpdateGrade(GradeViewModel vm);
        Task<ResponseService> CreateGrade(GradeViewModel vm);
        Task<ResponseService> DeleteGrade(int id);
        Task<List<GradeViewModel>> GetAllGrades();

        // CRUD operations for Subjects
        Task<ResponseService> UpdateSubject(SubjectViewModel vm);
        Task<ResponseService> CreateSubject(SubjectViewModel vm);
        Task<ResponseService> DeleteSubject(int id);
        Task<List<SubjectViewModel>> GetAllSubjects();
    }
}
