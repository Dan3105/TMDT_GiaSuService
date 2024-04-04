using GiaSuService.Models.UtilityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.EmployeeViewModel
{
    public class TutorRequestProfileEditViewModel
    {
        public int RequestId { get; set; }
        [Required]
        public int NStudents { get; set; }

        public string? AdditionalDetail { get; set; } = string.Empty;
        
        [Required]
        public string Addressdetail { get; set; } = string.Empty;

        public int ProvinceId { get; set; } = -1;
        public int DistrictId { get; set; } = -1;
        public int SubjectId { get; set; } = -1;
        public int GradeId { get; set; } = -1;

        public DateTime CreateDate { get; set; }
        public DateTime ExpireDate { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;

        public List<int> SelectedBefore { get; set; } = new List<int>();

        public List<ProvinceViewModel> Provinces { get; set; } = new List<ProvinceViewModel>();
        public List<SubjectViewModel> Subjects { get; set; } = new List<SubjectViewModel>();
        public List<GradeViewModel> Grades { get; set; } = new List<GradeViewModel>();
        public List<SessionViewModel> Sessions { get; set; } = new List<SessionViewModel>();

        public List<int> SessionSelected => Sessions.Where(p => p.IsChecked).Select(p => p.SessionId).ToList();

    }
}
