using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepo _tutorRepository;
        private readonly IStatusRepo _statusRepository;
        private readonly IProfileRepo _profileRepository;
        public TutorService(ITutorRepo tutorRepository, IStatusRepo statusRepository, IProfileRepo profileRepository)
        {
            _tutorRepository = tutorRepository;
            _statusRepository = statusRepository;
            _profileRepository = profileRepository;
        }
        public async Task<List<Tutor>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId)
        {
            List<Tutor> tutorprofiles = (await _tutorRepository.GetTutorprofilesByFilter(subjectId, districtId, gradeId)).ToList();
            return tutorprofiles;
        }

        public async Task<List<Tutor>> GetTutorprofilesByClassId(int classId)
        {
            List<Tutor> tutorprofiles = (await _tutorRepository.GetTutorprofilesByClassId(classId)).ToList();
            return tutorprofiles;
        }

        public async Task<List<TutorRegisterViewModel>> GetRegisterTutorOnPending(int page)
        {
            List<TutorRegisterViewModel> tutorprofiles = await _tutorRepository.GetRegisterTutorOnPending(page);
            return tutorprofiles;
        }

        public async Task<TutorProfileViewModel?> GetTutorprofileById(int id)
        {
            string? current_status = await _statusRepository.GetLatestStatusInTutorRegister(id);
            if (current_status == null)
            {
                return null;
            }
            TutorProfileViewModel? tutorprofile = await _profileRepository.GetTutorProfile(id);
            if(tutorprofile == null)
            {
                return null;
            }

            tutorprofile.Formstatus = current_status;
            return tutorprofile;
        }

        public async Task<List<Tutor>> GetSubTutors(List<int> ids)
        {
            return await _tutorRepository.GetSubTutorProfile(ids);
        }

        public async Task<ResponseService> UpdateTutorProfileStatus(int tutorId, string status)
        {
            var dbStatus = await _statusRepository.GetStatus(status, AppConfig.register_status);
            if (dbStatus == null)
            {
                return new ResponseService { Message = "Không cập nhật được trạng thái vui lòng làm lại ", Success = false };
            }

            Tutor? tutor = await _tutorRepository.GetTutor(tutorId);
            if (tutor == null)
            {
                return new ResponseService { Message = "Không tìm được gia sư này ", Success = false };
            }

            if (dbStatus.Name == AppConfig.RegisterStatus.APPROVAL.ToString().ToLower() ||
                dbStatus.Name == AppConfig.RegisterStatus.UPDATE.ToString().ToLower())
            {
                tutor.Isvalid = true;
            }
            else
            {
                tutor.Isvalid = false;
            }

            tutor.Registerstatusdetails.Add(new Registerstatusdetail
            {
                Context = "",
                Reviewdate = DateOnly.FromDateTime(DateTime.Now),
                Status = dbStatus,
            });

            bool isSuccess = await _tutorRepository.UpdateTutor(tutor);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Cập nhật thành công" };
            }
            else
            {
                return new ResponseService { Success = false, Message = "Lỗi hệ thống không cập nhật được" };
            }
        }
    }
}
