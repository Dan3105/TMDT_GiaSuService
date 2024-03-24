using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;
        public TutorService(ITutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }
        public async Task<List<Tutorprofile>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId)
        {
            List<Tutorprofile> tutorprofiles = (await _tutorRepository.GetTutorprofilesByFilter(subjectId, districtId, gradeId)).ToList();
            return tutorprofiles;
        }

        public async Task<List<Tutorprofile>> GetTutorprofilesByClassId(int classId)
        {
            List<Tutorprofile> tutorprofiles = (await _tutorRepository.GetTutorprofilesByClassId(classId)).ToList();
            return tutorprofiles;
        }

        public async Task<List<Tutorprofile>> GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus status)
        {
            List<Tutorprofile> tutorprofiles = (await _tutorRepository.GetTutorprofilesByRegisterStatus(status)).ToList();
            return tutorprofiles;
        }

        public async Task<bool> UpdateTutorprofile(Tutorprofile tutor)
        {
            try
            {
                return await _tutorRepository.UpdateProfile(tutor);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTutorprofileStatus(int tutorId, AppConfig.RegisterStatus status)
        {
            try
            {
                return await _tutorRepository.UpdateRegisterStatus(tutorId, status);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
