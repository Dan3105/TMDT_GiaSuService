using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorRequestFormService
    {
        public Task<Tutorrequestform?> GetTutorRequestFormById(int formId);
        //public Task<List<Tutorrequestform>> GetTutorrequestforms(AppConfig.TutorRequestStatus status);
        public Task<ResponseService> CreateForm(Tutorrequestform form, List<int> sessionList, List<int> tutorList);
        public Task<bool> UpdateForm(Tutorrequestform form, List<int> sessionList, string statusName);

    }
}
