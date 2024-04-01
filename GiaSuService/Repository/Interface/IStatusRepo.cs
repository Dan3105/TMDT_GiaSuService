namespace GiaSuService.Repository.Interface
{
    //REMEMBER LOWER CASE BEFORE ADD
    public interface IStatusRepo
    {
        public Task<List<string>> GetAllStatus(string typeStatus);
        public Task<int?> GetStatusId(string nameStatus, string typeStatus);
    }
}
