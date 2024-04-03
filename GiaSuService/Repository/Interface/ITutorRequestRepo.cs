using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRequestRepo : IRepository<Tutorrequestform>
    {
        public Task<Tutorrequestform?> Get(int id);
        public Task<List<Tutorrequestform>> GetAll();
        public Task<List<Tutorrequestform>> GetByFilter(int subjectId, 
            int gradeId, int districtId);
        //public Task<List<Tutorrequestform>> GetByStatus(AppConfig.TutorRequestStatus status);
    }
}
