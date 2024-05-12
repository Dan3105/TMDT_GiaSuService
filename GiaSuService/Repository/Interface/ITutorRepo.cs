using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRepo
    {
        //if param is an empty string it will get all TutorProfiles
        public Task<IEnumerable<AccountListViewModel>> GetTutorAccountsByFilter(
            int subjectId, int districtId, int gradeId, int page);
        public Task<PageCardListViewModel> GetTutorCardsByFilter(
            int subjectId, int districtId, int gradeId, int page);
        public Task<IEnumerable<Tutor>> GetTutorprofilesByClassId(int classId);
        public Task<List<TutorCardViewModel>> GetSubTutorCardView(List<int> ids);
        public Task<PageTutorRegisterListViewModel> GetRegisterTutorOnPending(int page, RegisterStatus status);

        public Task<Tutor?> GetTutor(int id);

//Only update status 
        public Task<bool> UpdateTutor(Tutor tutor);
        public Task<bool> SaveChanges();

        public Task<TutorStatusDetail?> GetLatestTutorStatusDetail(int tutorId);

        //This code not load the Differnce and Orignal so be careful
        public Task<TutorProfileStatusDetailHistoryViewModel?> GetATutorProfileHistoryDetail(int statusDetail);

        //This code not load the Difference and Original detail so be careful
        public Task<IEnumerable<TutorProfileStatusDetailHistoryViewModel>> GetTutorProfilesHistoryDetail(int tutorId);

        public Task<List<TutorApplyCardViewModel>> GetListTutorApplyForm(int tutorId);
        public Task<RequestTutorApplyDetailViewModel?> GetRequestTutorApplyDetail(int requestId, int tutorId);
    }
}
