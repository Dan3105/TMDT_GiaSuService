using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Diagnostics;

namespace GiaSuService.Services
{
    public class TutorRequestFormService : ITutorRequestFormService
    {
        private readonly ITutorRequestFormRepository _repo;
        //public TutorRequestFormService(ITutorRequestFormRepository repo)
        //{
        //    _repo = repo;
        //}

        public async Task<List<Tutorrequestform>> GetTutorRequestFormsByFilter(
           int subjectId, int districtId, int gradeId)
        {
            List<Tutorrequestform> forms = (await _repo.GetByFilter(subjectId, gradeId, districtId)).ToList();
            return forms;
        }

        public async Task<Tutorrequestform?> GetTutorRequestFormById(int formId)
        {
            Tutorrequestform? form = (await _repo.Get(formId));
            return form;
        }

        public async Task<List<Tutorrequestform>> GetTutorRequestForms()
        {
            List<Tutorrequestform> forms = (await _repo.GetAll()).ToList();
            return forms;
        }

        public async Task<bool> CreateForm(Tutorrequestform form)
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

        public async Task<bool> UpdateForm(Tutorrequestform form)
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

        //public async Task<List<Tutorrequestform>> GetTutorrequestforms(AppConfig.TutorRequestStatus status)
        //{
        //    return await _repo.GetByStatus(status);
        //}
    }
}
