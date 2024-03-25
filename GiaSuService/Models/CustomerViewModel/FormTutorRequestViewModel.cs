using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Models.CustomerViewModel
{
    public class FormTutorRequestViewModel
    {
        public TutorRequestProfile Profile { get; set; } = new TutorRequestProfile();

        public List<ProvinceViewModel> Provinces { get; set; } = new List<ProvinceViewModel>();
        public List<SessionViewModel> Sessions { get; set; } = new List<SessionViewModel>();
        public List<SubjectViewModel> Subjects { get; set; } = new List<SubjectViewModel>();
        public List<GradeViewModel> Grades { get; set; } = new List<GradeViewModel>();

    }

    public class TutorRequestProfile
    {
        public int NStudents { get; set; }
        public int NSessions { get; set; }
        public string AdditionalDetail { get; set; } = string.Empty;
        public string Addressdetail { get; set; } = string.Empty;
        public int DistrictId { get; set; } = -1;
        public int SessionId { get; set; } = -1;
        public int SubjectId { get; set; } = -1;
        public int GradeId { get; set; } = -1;
    }
}
