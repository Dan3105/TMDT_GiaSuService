using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepo _tutorRepository;
        //public TutorService(ITutorRepository tutorRepository)
        //{
        //    _tutorRepository = tutorRepository;
        //}
        public async Task<List<Tutor>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId)
        {
            List<Tutor> tutorprofiles = (await _tutorRepository.GetTutorprofilesByFilter(subjectId, districtId, gradeId)).ToList();
            return tutorprofiles;
        }

        public async Task<List<Tutor>> GetTutorprofilesByClassId(int classId)
        {
            List<Tutor> tutorprofiles = (await _tutorRepository.GetTutorprofilesByClassId(classId)).ToList();
            return tutorprofiles;
        }

        //public async Task<List<Tutorprofile>> GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus status)
        //{
        //    List<Tutorprofile> tutorprofiles = (await _tutorRepository.GetTutorprofilesByRegisterStatus(status)).ToList();
        //    return tutorprofiles;
        //}

        public async Task<bool> UpdateTutorprofile(Tutor tutor)
        {
            try
            {
                //return await _tutorRepository.UpdateProfile(tutor);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTutorprofileStatus(int tutorId)
        {
            try
            {
                //return await _tutorRepository.UpdateRegisterStatus(tutorId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Tutor> GetTutorprofileById(int id)
        {
            //Tutor? tutorprofile = await _tutorRepository.GetTutorprofile(id);
            //return tutorprofile!;
            return null!;
        }

        public async Task<Tutor> GetTutorprofileByAccountId(int accountId)
        {
            Tutor? tutorprofile = await _tutorRepository.GetTutorprofileByAccountId(accountId);
            return tutorprofile!;
        }

        public async Task<List<Tutor>> GetSubTutors(List<int> ids)
        {
            return await _tutorRepository.GetSubTutorProfile(ids);
        }
    }
}
