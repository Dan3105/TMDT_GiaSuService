using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ICategoryRepo _categoryRepo;

        public CatalogService(ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        //public CatalogService(ISessionRepository sessionRepository, IGradeRepository gradeRepository, ISubjectRepository subjectRepository)
        //{
        //    _sessionRepository = sessionRepository;
        //    _gradeRepository = gradeRepository;
        //    _subjectRepository = subjectRepository;
        //}

        #region CRUD Session

        public async Task<List<SessionViewModel>> GetAllSessions()
        {
            return await _categoryRepo.GetAllSessions();
        }

        public async Task<SessionViewModel> GetSessionById(int sessionId)
        {
            return await _categoryRepo.GetSessionById(sessionId);
        }

       
        #endregion

        #region CRUD Grade
        public async Task<List<GradeViewModel>> GetAllGrades()
        {
            return await _categoryRepo.GetAllGrades();
        }

        public async Task<GradeViewModel> GetGradeById(int gradeId)
        {
            return await _categoryRepo.GetGradeById(gradeId);
        }
        #endregion

        #region CRUD Subject
        public async Task<List<SubjectViewModel>> GetAllSubjects()
        {
            return await _categoryRepo.GetAllSubjects();
        }

        public async Task<SubjectViewModel> GetSubjectById(int subjectId)
        {
            return await _categoryRepo.GetSubjectById(subjectId);
        }
        #endregion
    }
}
