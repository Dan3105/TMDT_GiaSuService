﻿using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.CustomerViewModel
{
    public class FormTutorRequestViewModel
    {
        public TutorRequestProfile Profile { get; set; } = new TutorRequestProfile();

        public List<ProvinceViewModel> Provinces { get; set; } = new List<ProvinceViewModel>();
        public List<SubjectViewModel> Subjects { get; set; } = new List<SubjectViewModel>();
        public List<GradeViewModel> Grades { get; set; } = new List<GradeViewModel>();
        public List<SessionViewModel> Sessions { get; set; } = new List<SessionViewModel>();

        public List<int> SessionSelected => Sessions.Where(p =>p.IsChecked).Select(p => p.SessionId).ToList();
    }

    public class TutorRequestProfile
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Số lượng học sinh phải lớn hơn 1 và bé hơn 6")]
        public int NStudents { get; set; }

        public string? AdditionalDetail { get; set; } = string.Empty;

        [Required]
        public string Addressdetail { get; set; } = string.Empty;

        public int SelectedProvinceId { get; set; } = -1;
        public int DistrictId { get; set; } = -1;
        public int SubjectId { get; set; } = -1;
        public int GradeId { get; set; } = -1;

        public DateTime CreateDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
