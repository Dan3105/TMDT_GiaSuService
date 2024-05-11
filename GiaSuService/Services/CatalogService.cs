using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;

namespace GiaSuService.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly IStatusRepo _statusRepo;

        public CatalogService(ICategoryRepo categoryRepo, IStatusRepo statusRepo)
        {
            _categoryRepo = categoryRepo;
            _statusRepo = statusRepo;
        }

        #region CRUD Session

        public async Task<List<SessionViewModel>> GetAllSessions()
        {
            return await _categoryRepo.GetAllSessions();
        }

        public async Task<ResponseService> UpdateSessionDate(SessionViewModel vm)
        {
            SessionViewModel? session = await _categoryRepo.GetSessionById(vm.SessionId);

            if (session == null)
            {
                return new ResponseService { Message = "Không tìm thấy dữ liệu", Success = false };
            }

            bool isUnique = await _categoryRepo.IsUniqueName(vm.SessionName, vm.GetType());

            if (!isUnique && (!session.SessionName.Equals(vm.SessionName)
                && session.Value != vm.Value))
            {
                return new ResponseService { Message = "Tên bị trùng trong hệ thống", Success = false };
            }

            bool updateSuccess = await _categoryRepo.UpdateSessionDate(vm);
            if (updateSuccess)
            {
                return new ResponseService { Message = "Cập nhật thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }
        public async Task<ResponseService> CreateSessionDate(SessionViewModel vm)
        {
            bool isUnique = await _categoryRepo.IsUniqueName(vm.SessionName, vm.GetType());
            if (!isUnique)
            {
                return new ResponseService { Message = "Tên bị trùng trong hệ thống", Success = false };
            }

            bool addSuccess = await _categoryRepo.CreateSessionDate(vm);
            if (addSuccess)
            {
                return new ResponseService { Message = "Tạo thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }
        public async Task<ResponseService> DeleteSessionDate(int id)
        {
            bool delSuccess = await _categoryRepo.DeleteSessionDate(id);
            if (delSuccess)
            {
                return new ResponseService { Message = "Xóa thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }


        #endregion

        #region CRUD Subject

        public async Task<ResponseService> UpdateSubject(SubjectViewModel vm)
        {
            SubjectViewModel? session = await _categoryRepo.GetSubjectById(vm.SubjectId);

            if (session == null)
            {
                return new ResponseService { Message = "Không tìm thấy dữ liệu", Success = false };
            }

            bool isUnique = await _categoryRepo.IsUniqueName(vm.SubjectName, vm.GetType());

            if (!isUnique && (!session.SubjectName.Equals(vm.SubjectName)
                && session.Value != vm.Value))
            {
                return new ResponseService { Message = "Tên bị trùng trong hệ thống", Success = false };
            }

            bool updateSuccess = await _categoryRepo.UpdateSubject(vm);
            if (updateSuccess)
            {
                return new ResponseService { Message = "Cập nhật thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }
        public async Task<ResponseService> CreateSubject(SubjectViewModel vm)
        {
            bool isUnique = await _categoryRepo.IsUniqueName(vm.SubjectName, vm.GetType());
            if (!isUnique)
            {
                return new ResponseService { Message = "Tên bị trùng trong hệ thống", Success = false };
            }

            bool addSuccess = await _categoryRepo.CreateSubject(vm);
            if (addSuccess)
            {
                return new ResponseService { Message = "Tạo thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }
        public async Task<ResponseService> DeleteSubject(int id)
        {
            bool delSuccess = await _categoryRepo.DeleteSubject(id);
            if (delSuccess)
            {
                return new ResponseService { Message = "Xóa thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }

        public async Task<List<SubjectViewModel>> GetAllSubjects()
        {
            return await _categoryRepo.GetAllSubjects();
        }
        #endregion

        #region CRUD Grade
        public async Task<List<GradeViewModel>> GetAllGrades()
        {
            return await _categoryRepo.GetAllGrades();
        }
        public async Task<ResponseService> UpdateGrade(GradeViewModel vm)
        {
            GradeViewModel? grade = await _categoryRepo.GetGradeById(vm.GradeId);

            if (grade == null)
            {
                return new ResponseService { Message = "Không tìm thấy dữ liệu", Success = false };
            }

            bool isUnique = await _categoryRepo.IsUniqueName(vm.GradeName, vm.GetType());

            if (!isUnique && (!grade.GradeName.Equals(vm.GradeName)
                && grade.Value != vm.Value))
            {
                return new ResponseService { Message = "Tên bị trùng trong hệ thống", Success = false };
            }

            bool updateSuccess = await _categoryRepo.UpdateGrade(vm);
            if (updateSuccess)
            {
                return new ResponseService { Message = "Cập nhật thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }
        public async Task<ResponseService> CreateGrade(GradeViewModel vm)
        {
            bool isUnique = await _categoryRepo.IsUniqueName(vm.GradeName, vm.GetType());
            if (!isUnique)
            {
                return new ResponseService { Message = "Tên bị trùng trong hệ thống", Success = false };
            }

            bool addSuccess = await _categoryRepo.CreateGrade(vm);
            if (addSuccess)
            {
                return new ResponseService { Message = "Tạo thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }
        public async Task<ResponseService> DeleteGrade(int id)
        {
            bool delSuccess = await _categoryRepo.DeleteGrade(id);
            if (delSuccess)
            {
                return new ResponseService { Message = "Xóa thành công", Success = true };
            }

            return new ResponseService { Message = "Lỗi hệ thống", Success = false };
        }


        #endregion

        #region CRUD TutorType
        public async Task<List<TutorTypeViewModel>> GetAllTutorType()
        {
            return await _categoryRepo.GetAllTutorTypes();
        }

        public async Task<TutorTypeViewModel> GetTutorTypeById(int tutorTypeId)
        {
            return await _categoryRepo.GetTutorTypeById(tutorTypeId);
        }

        #endregion
        
        public async Task<List<StatusNamePair>> GetAllStatus(string statusType)
        {
            return await _statusRepo.GetAllStatus(statusType);
        }
    }
}
