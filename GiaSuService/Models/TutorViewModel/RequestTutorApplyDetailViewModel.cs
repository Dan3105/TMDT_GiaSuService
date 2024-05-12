using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Models.TutorViewModel
{
    public class RequestTutorApplyDetailViewModel
    {
        public int RequestId { get; set; }
        public int TutorId { get; set; }

        public int NStudent {  get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;     // House number and street name
        public string Location { get; set; } = string.Empty;    // Information about district and province
        public string AdditionalDetail {  get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string RequestStatus { get; set; } = string.Empty;
        public string RequestStatusDescription { get; set; } = string.Empty;
        public string QueueStatus { get; set; } = string.Empty;
        public string QueueStatusDescription { get; set; } = string.Empty;
    }
}
