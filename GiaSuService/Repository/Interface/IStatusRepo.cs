using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;

namespace GiaSuService.Repository.Interface
{
    //REMEMBER LOWER CASE BEFORE ADD
    public interface IStatusRepo
    {
        public Task<List<StatusNamePair>> GetAllStatus(string typeStatus);
        public Task<Status?> GetStatus(string nameStatus, string typeStatus);
        public Task<string?> GetLatestStatusInTutorRegister(int tutorId, bool isTranslator);

    }
}
