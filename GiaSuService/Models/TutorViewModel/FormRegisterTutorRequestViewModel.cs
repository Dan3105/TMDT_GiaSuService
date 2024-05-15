using GiaSuService.Configs;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.UtilityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.TutorViewModel
{
    public class FormRegisterTutorRequestViewModel
    {
        [Required]
        public RegisterTutorProfile RegisterTutorProfile { get; set; } = new RegisterTutorProfile();

        [Required]
        public RegisterAccountProfileViewModel AccountProfile { get; set; } = new RegisterAccountProfileViewModel();

        public bool ConfirmBox { get; set; }
        public List<ProvinceViewModel> ListProvince { get; set; } = new List<ProvinceViewModel>();
        public List<SessionViewModel> ListSessionDate { get; set; } = new List<SessionViewModel>();
        public List<GradeViewModel> ListGrade { get; set; } = new List<GradeViewModel>();
        public List<SubjectViewModel> ListSubject { get; set; } = new List<SubjectViewModel>();
        public List<TutorTypeViewModel> ListTutorType { get; set; } = new List<TutorTypeViewModel>();
        public List<int> ListDistrict { get; set; } = new List<int>();

        public List<SessionViewModel> GetSessionSelected => ListSessionDate.Where(p => p.IsChecked).ToList();
        public List<GradeViewModel> GetGradeSelected => ListGrade.Where(p => p.IsChecked).ToList();
        public List<SubjectViewModel> GetSubjectSelected => ListSubject.Where(p => p.IsChecked).ToList();

        public FormRegisterTutorRequestViewModel() { }
    }

    public class RegisterTutorProfile
    {
        [Required(ErrorMessage = "Vui lòng nhập tên trường")]
        public string College { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập chuyên ngành")]
        public string Area { get; set; } = string.Empty;

        public int TypeTutorId { get; set; }

        public string? AdditionalInfo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập năm bắt đầu")]
        public short AcademicYearFrom { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập năm kết thúc")]
        public short AcademicYearto { get; set; }

        public RegisterTutorProfile() { }

    }
}
