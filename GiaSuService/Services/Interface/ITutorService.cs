using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorService
    {
        //if all id in param equal to 0 it will get all TutorProfiles
        public Task<List<Tutorprofile>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId);
        public Task<List<Tutorprofile>> GetTutorprofilesByClassId(int classId);
        public Task<List<Tutorprofile>> GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus status);
        public Task<bool> UpdateTutorprofile(Tutorprofile tutor);
        public Task<bool> UpdateTutorprofileStatus(int tutorId, AppConfig.RegisterStatus status);

    }
}
