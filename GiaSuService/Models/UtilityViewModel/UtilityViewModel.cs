using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.UtilityViewModel
{
    public class SessionViewModel
    {
        public string SessionName { get; set; } = string.Empty;
        [Required]
        public int SessionId { get; set; }
        public bool IsChecked { get; set; } = false;
    }

    public class GradeViewModel
    {
        public string GradeName { get; set; } = string.Empty;
        [Required]
        public int GradeId { get; set; }
        public bool IsChecked { get; set; } = false;

    }

    public class SubjectViewModel
    {
        public string SubjectName { get; set; } = string.Empty;
        [Required]
        public int SubjectId { get; set; }
        public bool IsChecked { get; set; } = false;

    }

    public class DistrictViewModel
    {
        [Required]
        public int DistrictId { get; set; }

        public string DistrictName { get; set; } = "";

        [Required]
        public bool IsChecked { get; set; } 
    }

    public class ProvinceViewModel
    {
        [Required]
        public int ProvinceId { get; set; }

        public required string ProvinceName { get; set; }
    }

}
