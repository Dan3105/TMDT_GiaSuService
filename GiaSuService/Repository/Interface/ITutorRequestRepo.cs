using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRequestRepo : IRepository<RequestTutorForm>
    {
        public Task<RequestTutorForm?> Get(int id);

        public Task<List<RequestTutorForm>> GetAll();
        public Task<List<RequestTutorForm>> GetByFilter(int subjectId, 
            int gradeId, int districtId);
        public Task<PageTutorRequestQueueListViewModel> GetTutorRequestQueueByStatus(int statusId, int page);
        public Task<PageTutorRequestListViewModel> GetTutorRequestCardByStatus(
            int provinceId, int districtId, int subjectId, int gradeId, int statusId, int page);
        public Task<PageTutorRequestListViewModel> GetTutorRequestCardByStatus(
            int provinceId, int districtId, int subjectId, int gradeId, int statusId, int page, int tutorId);
        public Task<TutorRequestCardViewModel?> GetTutorRequestCardById(int requestId);

        public Task<TutorRequestProfileViewModel?> GetTutorRequestProfile(int id);
        public Task<TutorRequestProfileEditViewModel?> GetTutorRequestProfileEdit(int id);
        public Task<bool> UpdateTutorRequestProfileEdit(TutorRequestProfileEditViewModel model);

        public Task<List<CustomerTutorRequestViewModel>> GetListTutorRequestOfCustomer(int customerId);

        public Task<bool> UpdateTutorApplyStatus(int tutorId, int requestId, Status status);
    }
}
