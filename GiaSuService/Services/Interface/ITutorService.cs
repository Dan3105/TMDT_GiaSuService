using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorService
    {
        //if all id in param equal to 0 it will get all TutorProfiles
        public Task<List<Tutor>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId);
        public Task<List<Tutor>> GetTutorprofilesByClassId(int classId);
        public Task<List<TutorRegisterViewModel>> GetRegisterTutorOnPending(int page);
        public Task<TutorProfileViewModel?> GetTutorprofileById(int id);
        public Task<List<Tutor>> GetSubTutors(List<int> ids);

        public Task<ResponseService> UpdateTutorProfileStatus(int tutorId, string status);
    }
}
