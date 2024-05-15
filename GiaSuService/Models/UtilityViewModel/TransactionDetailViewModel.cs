namespace GiaSuService.Models.UtilityViewModel
{
    public class TransactionDetailViewModel
    {
        public int TransactionId { get; set; }
        public int TutorId { get; set; }
        public int RequestId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string TutorName { get; set; } = string.Empty;
        public string CreateDate { get; set; } = string.Empty;
        public string PaymentDate { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Context { get; set; } = string.Empty;
        public string RequestStatus { get; set; } = string.Empty;
        public string QueueStatus {  get; set; } = string.Empty;
        public string TransactionType {  get; set; } = string.Empty;
    }
}
