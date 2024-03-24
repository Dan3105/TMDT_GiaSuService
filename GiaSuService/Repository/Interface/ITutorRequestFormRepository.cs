using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRequestFormRepository : IRepository<Tutorrequestform>
    {
        public Task<Tutorrequestform?> Get(int id);
        public Task<IEnumerable<Tutorrequestform>> GetAll();
        public Task<IEnumerable<Tutorrequestform>> GetByFilter(int subjectId, 
            int gradeId, int districtId);
    }
}
