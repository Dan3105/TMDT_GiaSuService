using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorService
    {
        //if param is an empty string it will get all TutorProfiles
        public Task<List<Tutorprofile>> GetTutorprofilesByFilter(
            string subject, string district, string grade);
        public Task<List<Tutorprofile>> GetTutorprofilesByClassId(int classId);
        public Task<bool> UpdateTutorprofile(Tutorprofile tutor);
        public Task<bool> UpdateTutorprofileStatus(int tutorId, AppConfig.RegisterStatus status);

    }
}
