using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.UtilityViewModel
{
    public class SessionViewModel
    {
        public string SessionName { get; set; } = string.Empty;
        public int SessionId { get; set; }
        public bool IsChecked { get; set; } = false;

        public int Value { get; set; }
    }

    public class GradeViewModel
    {
        public string GradeName { get; set; } = string.Empty;
        public int GradeId { get; set; }
        public bool IsChecked { get; set; } = false;

        public int Value { get; set; }
        public decimal Fee { get; set; }
    }

    public class SubjectViewModel
    {
        public string SubjectName { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public bool IsChecked { get; set; } = false;
        
        public int Value { get; set; }
    }

    public class DistrictViewModel
    {
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public bool IsChecked { get; set; } 
    }

    public class ProvinceViewModel
    {
        public int ProvinceId { get; set; }
        public required string ProvinceName { get; set; }
    }

    public class TutorTypeViewModel
    {
        public int TutorTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
