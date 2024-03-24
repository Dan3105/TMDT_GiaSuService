using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorRequestFormService
    {
        public Task<List<Tutorrequestform>> GetTutorRequestFormsByFilter(
            int subjectId, int districtId, int gradeId);
        public Task<Tutorrequestform?> GetTutorRequestFormById(int formId);
        public Task<List<Tutorrequestform>> GetTutorRequestForms();
        public Task<bool> CreateForm(Tutorrequestform form);
        public Task<bool> UpdateForm(Tutorrequestform form);

    }
}
