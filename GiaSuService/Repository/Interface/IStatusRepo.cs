using GiaSuService.EntityModel;

namespace GiaSuService.Repository.Interface
{
    //REMEMBER LOWER CASE BEFORE ADD
    public interface IStatusRepo
    {
        public Task<List<string>> GetAllStatus(string typeStatus);
        public Task<Status?> GetStatus(string nameStatus, string typeStatus);
        public Task<string?> GetLatestStatusInTutorRegister(int tutorId);

    }
}
