namespace GiaSuService.Models.EmployeeViewModel
{
    public class TutorRequestQueueViewModel
    {
        public int FormId { get; set; }
        public string FullNameRequester { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string AddressName { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
    }
}