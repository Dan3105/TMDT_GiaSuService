namespace GiaSuService.Models.TutorViewModel
{
    public class TutorRequestCardViewModel
    {
        public int RequestId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string AdditionalDetail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string SessionsCanTeach {  get; set; } = string.Empty;
        public string RequestStatus {  get; set; } = string.Empty;
    }
}
