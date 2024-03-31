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
        public List<TutorCardViewModel> TutorCards { get; set; } = new List<TutorCardViewModel>();
    }

    public class TutorRequestProfile
    {
        [Required]
        public int NStudents { get; set; }

        [Required]
        public int NSessions { get; set; }

        public string? AdditionalDetail { get; set; } = string.Empty;

        [Required]
        public string Addressdetail { get; set; } = string.Empty;

        public int DistrictId { get; set; } = -1;
        public int SubjectId { get; set; } = -1;
        public int GradeId { get; set; } = -1;
    }
}