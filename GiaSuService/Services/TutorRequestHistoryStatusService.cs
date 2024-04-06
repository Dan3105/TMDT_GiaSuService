using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TutorRequestHistoryStatusService : ITutorRequestHistoryStatusService
    {
        private readonly ITutorRequestHistoryStatusRepo _repo;
        //public TutorRequestHistoryStatusService(ITutorRequestHistoryStatusRepository repo)
        //{
        //    _repo = repo;
        //}

        public async Task<bool> CreateRequestHistory(RequestStatus form)
        {
            try
            {
                _repo.Create(form);
                var isSucced = await _repo.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<RequestStatus>> GetByTutorRequestFormId(int reqFormId)
        {
            List<RequestStatus> forms = (await _repo.GetByTutorRequestFormId(reqFormId)).ToList();
            return forms;
        }

        public async Task<bool> UpdateRequestHistory(RequestStatus form)
        {
            try
            {
                _repo.Update(form);
                var isSucced = await _repo.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
