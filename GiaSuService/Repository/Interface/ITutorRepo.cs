using GiaSuService.Configs;
using GiaSuService.EntityModel;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRepo
    {
        //if param is an empty string it will get all TutorProfiles
        public Task<IEnumerable<Tutor>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId);
        public Task<IEnumerable<Tutor>> GetTutorprofilesByClassId(int classId);
        public Task<Tutor?> GetTutorprofileByAccountId(int accountId);
        public Task<List<Tutor>> GetSubTutorProfile(List<int> ids);
        
    }
}
