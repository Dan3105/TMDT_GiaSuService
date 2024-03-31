using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorService
    {
        //if all id in param equal to 0 it will get all TutorProfiles
        public Task<List<Tutor>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId);
        public Task<List<Tutor>> GetTutorprofilesByClassId(int classId);
        //public Task<List<Tutor>> GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus status);
        public Task<bool> UpdateTutorprofile(Tutor tutor);
        public Task<bool> UpdateTutorprofileStatus(int tutorId);
        public Task<Tutor> GetTutorprofileById(int id);
        public Task<Tutor> GetTutorprofileByAccountId(int accountId);
        public Task<List<Tutor>> GetSubTutors(List<int> ids);
    }
}
