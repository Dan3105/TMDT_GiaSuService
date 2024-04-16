using GiaSuService.Configs;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Services.Interface
{
    public interface ITutorService
    {
        //if all id in param equal to 0 it will get all TutorProfiles
        public Task<List<AccountListViewModel>> GetTutorAccountsByFilter(
            int subjectId, int districtId, int gradeId, int page);

        public Task<List<TutorCardViewModel>> GetTutorCardsByFilter(
            int subjectId, int districtId, int gradeId, int page);

        public Task<List<TutorRegisterViewModel>> GetRegisterTutoByStatus(int page, RegisterStatus status);
        public Task<TutorProfileViewModel?> GetTutorprofileById(int id);
        public Task<List<TutorCardViewModel>> GetSubTutors(List<int> ids);

        public Task<ResponseService> UpdateTutorProfileStatus(int tutorId, string status, string context);
        public Task<DifferenceUpdateRequestFormViewModel?> GetTutorUpdateRequest(int tutorId);

        public Task<IEnumerable<TutorProfileStatusDetailHistoryViewModel>> GetStatusTutorHistory(int tutorId);
        public Task<TutorProfileStatusDetailHistoryViewModel?> GetAStatusTutorHistory(int historyId);

        public Task<ResponseService> ApplyRequest(int tutorId, int requestId);
        public Task<ResponseService> CancelApplyRequest(int tutorId, int requestId);

        public Task<List<TutorApplyFormViewModel>> GetTutorApplyForm(int tutorId);

        public Task<RequestTutorApplyDetailViewModel?> GetTutorRequestProfileById(int requestId);
    }
}
