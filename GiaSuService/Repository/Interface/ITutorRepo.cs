using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using Microsoft.EntityFrameworkCore;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRepo
    {
        //if param is an empty string it will get all TutorProfiles
        public Task<IEnumerable<Tutor>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId);
        public Task<IEnumerable<Tutor>> GetTutorprofilesByClassId(int classId);
        public Task<List<Tutor>> GetSubTutorProfile(List<int> ids);
        public Task<List<TutorRegisterViewModel>> GetRegisterTutorOnPending(int page);


        public Task<Tutor?> GetTutor(int id);
        public Task<bool> UpdateTutor(Tutor tutor);
        public Task<bool> SaveChanges();

    }
}
