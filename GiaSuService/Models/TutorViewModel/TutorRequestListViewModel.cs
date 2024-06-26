﻿using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Models.TutorViewModel
{
    public class TutorRequestListViewModel
    {
        public List<ProvinceViewModel> ProvinceList { get; set; } = new List<ProvinceViewModel>();
        public List<GradeViewModel> GradeList { get; set; } = new List<GradeViewModel>() { };
        public List<SubjectViewModel> SubjectList { get; set; } = new List<SubjectViewModel>() { };
    
        public int SelectedProvinceId { get; set; }
        public int SelectedDistrictId { get; set; }
    }
}
