using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRequestRepo : IRepository<Tutorrequestform>
    {
        public Task<Tutorrequestform?> Get(int id);

        public Task<List<Tutorrequestform>> GetAll();
        public Task<List<Tutorrequestform>> GetByFilter(int subjectId, 
            int gradeId, int districtId);
        public Task<List<TutorRequestQueueViewModel>> GetTutorRequestQueueByStatus(int statusId, int page);

        public Task<TutorRequestProfileViewModel?> GetTutorRequestProfile(int id);
    }
}
