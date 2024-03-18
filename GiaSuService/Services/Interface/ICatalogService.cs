using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ICatalogService
    {
        // CRUD operations for Sessions
        Task<List<Sessiondate>> GetAllSessions();
        Task<Sessiondate> GetSessionById(int sessionId);
        Task<bool> CreateSession(Sessiondate session);
        Task<bool> UpdateSession(Sessiondate session);
        Task<bool> DeleteSession(Sessiondate session);

        // CRUD operations for Grades
        Task<List<Grade>> GetAllGrades();
        Task<Grade> GetGradeById(int gradeId);
        Task<bool> CreateGrade(Grade grade);
        Task<bool> UpdateGrade(Grade grade);
        Task<bool> DeleteGrade(Grade grade);

        // CRUD operations for Subjects
        Task<List<Subject>> GetAllSubjects();
        Task<Subject> GetSubjectById(int subjectId);
        Task<bool> CreateSubject(Subject subject);
        Task<bool> UpdateSubject(Subject subject);
        Task<bool> DeleteSubject(Subject subject);
    }
}
