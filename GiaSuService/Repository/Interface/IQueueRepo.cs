using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    public interface IQueueRepo
    {
        public bool AddTutorsToQueue(Tutorrequestform form, List<int> ids, int statusDefaultId);
        public Task<bool> SaveChanges();
    }
}
