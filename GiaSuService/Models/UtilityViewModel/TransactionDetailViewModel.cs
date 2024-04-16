namespace GiaSuService.Models.UtilityViewModel
{
    public class TransactionDetailViewModel
    {
        public int TutorId { get; set; }
        public int RequestId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string TutorName { get; set; } = string.Empty;
        public string CreateDate { get; set; } = string.Empty;
        public string PaymentDate { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
