namespace GiaSuService.Models.TutorViewModel
{
    public class TutorApplyCardViewModel
    {
        public int RequestId { get; set; }

        public int NStudents { get; set; }

        public DateTime? EnterDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;    // Information about district and province

        public string RequestStatus { get; set; } = string.Empty;
        public string RequestStatusDescription { get; set; } = string.Empty;
        public string QueueStatus { get; set; } = string.Empty;
        public string QueueStatusDescription { get; set; } = string.Empty;
    }
}
