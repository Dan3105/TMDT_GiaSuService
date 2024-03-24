using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TutorRequestHistoryStatusService : ITutorRequestHistoryStatusService
    {
        private readonly ITutorRequestHistoryStatusRepository _repo;
        public TutorRequestHistoryStatusService(ITutorRequestHistoryStatusRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> CreateRequestHistory(Tutorrequesthistorystatus form)
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

        public async Task<List<Tutorrequesthistorystatus>> GetByTutorRequestFormId(int reqFormId)
        {
            List<Tutorrequesthistorystatus> forms = (await _repo.GetByTutorRequestFormId(reqFormId)).ToList();
            return forms;
        }

        public async Task<bool> UpdateRequestHistory(Tutorrequesthistorystatus form)
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
