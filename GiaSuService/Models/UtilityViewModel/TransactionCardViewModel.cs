namespace GiaSuService.Models.UtilityViewModel
{
    public class TransactionCardViewModel
    {
        public int TransactionId { get; set; }
        public string CreateDate { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;    // Full name of employee created the transaction
        public decimal Price { get; set; }
        public string TransactionType {  get; set; } = string.Empty;    // PAID or REFUND
        public string PayStatus { get; set; } = string.Empty;
        
    }
}
