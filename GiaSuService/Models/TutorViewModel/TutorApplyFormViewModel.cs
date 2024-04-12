namespace GiaSuService.Models.TutorViewModel
{
    public class TutorApplyFormViewModel
    {
        public int RequestId { get; set; }
        public int Students { get; set; }

        public string? AdditionalDetail { get; set; }

        public DateTime? EnterDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
    }
}
