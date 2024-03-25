using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.TutorViewModel
{
    public class TutorCardViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Avatar { get;set; } =string.Empty;

        public string Birth { get; set; } = string.Empty;
        public string TutorType { get; set; } = string.Empty;
        public string College { get; set; } = string.Empty;
        public string Area {  get; set; } = string.Empty;
        public int GraduateYear { get; set; }
        public string GradeList { get; set; } = string.Empty;
        public string SubjectList { get; set; } = string.Empty;
        public string TeachingArea { get; set; } = string.Empty;
        public string AdditionalProfile { get; set; } = string.Empty;
    }
}
