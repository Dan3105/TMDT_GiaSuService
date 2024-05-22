using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Models.TutorViewModel
{
    public class TutorCardListViewModel
    {
        public List<ProvinceViewModel> ProvinceList { get; set; } = new List<ProvinceViewModel>();
        public List<GradeViewModel> GradeList { get; set; } = new List<GradeViewModel>() { };
        public List<SubjectViewModel> SubjectList { get; set; } = new List<SubjectViewModel>() { };
        //public List<TutorCardViewModel> TutorList { get; set; } = new List<TutorCardViewModel>() { };

        public int SelectedProvinceId { get; set; } = -1;
        public int SelectedDistrictId { get; set; } = -1;
    }
}
