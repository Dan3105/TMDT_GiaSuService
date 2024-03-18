using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ISubjectRepository _subjectRepository;

        public CatalogService(ISessionRepository sessionRepository, IGradeRepository gradeRepository, ISubjectRepository subjectRepository)
        {
            _sessionRepository = sessionRepository;
            _gradeRepository = gradeRepository;
            _subjectRepository = subjectRepository;
        }

        #region CRUD Session

        public async Task<List<Sessiondate>> GetAllSessions()
        {
            return await _sessionRepository.GetAllSessions();
        }

        public async Task<Sessiondate> GetSessionById(int sessionId)
        {
            return await _sessionRepository.GetSessionById(sessionId);
        }

        public async Task<bool> CreateSession(Sessiondate session)
        {
            try
            {
                _sessionRepository.Create(session);
                return await _sessionRepository.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateSession(Sessiondate session)
        {
            try
            {
                _sessionRepository.Update(session);
                return await _sessionRepository.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteSession(Sessiondate session)
        {
            try
            {
                _sessionRepository.Delete(session);
                return await (_sessionRepository.SaveChanges());
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region CRUD Grade
        public async Task<List<Grade>> GetAllGrades()
        {
            return await _gradeRepository.GetAllGrades();
        }

        public async Task<Grade> GetGradeById(int gradeId)
        {
            return await _gradeRepository.GetGradeById(gradeId);
        }

        public async Task<bool> CreateGrade(Grade grade)
        {
            try
            {
                _gradeRepository.Create(grade);
                return (await _gradeRepository.SaveChanges());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateGrade(Grade grade)
        {
            try
            {
                _gradeRepository.Update(grade);
                return (await _gradeRepository.SaveChanges());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteGrade(Grade grade)
        {
            try
            {
                _gradeRepository.Delete(grade);
                return await _gradeRepository.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region CRUD Subject
        public async Task<List<Subject>> GetAllSubjects()
        {
            return await _subjectRepository.GetAllSubjects();
        }

        public async Task<Subject> GetSubjectById(int subjectId)
        {
            return await _subjectRepository.GetSubjectById(subjectId);
        }

        public async Task<bool> CreateSubject(Subject subject)
        {
            try
            {
                _subjectRepository.Create(subject);
                return await (_subjectRepository.SaveChanges());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateSubject(Subject subject)
        {
            try
            {
                _subjectRepository.Update(subject);
                return (await _subjectRepository.SaveChanges());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteSubject(Subject subject)
        {
            try
            {
                _subjectRepository.Delete(subject);
                return await (_subjectRepository.SaveChanges());
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
