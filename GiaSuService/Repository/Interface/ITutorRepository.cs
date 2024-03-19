using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRepository : IRepository<Tutorprofile>
    {
        //if param is an empty string it will get all TutorProfiles
        public Task<IEnumerable<Tutorprofile>> GetTutorprofilesByFilter(
            string subject, string district, string grade);
        public Task<IEnumerable<Tutorprofile>> GetTutorprofilesByClassId(int classId);
        public Task<bool> UpdateProfile(Tutorprofile tutor);
        public Task<bool> UpdateRegisterStatus(int tutorProfileId, AppConfig.RegisterStatus status);
        
    }
}
