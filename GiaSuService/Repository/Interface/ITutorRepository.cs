using GiaSuService.Configs;
using GiaSuService.EntityModel;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRepository : IRepository<Tutor>
    {
        //if param is an empty string it will get all TutorProfiles
        public Task<IEnumerable<Tutor>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId);
        public Task<IEnumerable<Tutor>> GetTutorprofilesByClassId(int classId);
        public Task<Tutor?> GetTutorprofile(int id); //Get by tutorProfileId
        public Task<Tutor?> GetTutorprofileByAccountId(int accountId);
        //public Task<IEnumerable<Tutor>> GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus status);
        public Task<bool> UpdateProfile(Tutor tutor);
        //public Task<bool> UpdateRegisterStatus(int tutorProfileId, AppConfig.RegisterStatus status);
        public Task<List<Tutor>> GetSubTutorProfile(List<int> ids);
        
    }
}
