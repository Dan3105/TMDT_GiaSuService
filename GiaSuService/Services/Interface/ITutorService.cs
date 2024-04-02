using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorService
    {
        //if all id in param equal to 0 it will get all TutorProfiles
        public Task<List<AccountListViewModel>> GetTutorAccountsByFilter(
            int subjectId, int districtId, int gradeId, int page);

        public Task<List<TutorCardViewModel>> GetTutorCardsByFilter(
            int subjectId, int districtId, int gradeId, int page);

        public Task<List<TutorRegisterViewModel>> GetRegisterTutorOnPending(int page);
        public Task<TutorProfileViewModel?> GetTutorprofileById(int id);
        public Task<List<TutorCardViewModel>> GetSubTutors(List<int> ids);

        public Task<ResponseService> UpdateTutorProfileStatus(int tutorId, string status);
    }
}
