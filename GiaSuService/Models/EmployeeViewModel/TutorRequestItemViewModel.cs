namespace GiaSuService.Models.EmployeeViewModel
{
    public class TutorRequestItemViewModel
    {
        public int FormId { get; set; }
        public string FullNameRequester { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string AddressName { get; set; } = string.Empty;
        public DateOnly CreatedDate { get; set; }
    }
}
