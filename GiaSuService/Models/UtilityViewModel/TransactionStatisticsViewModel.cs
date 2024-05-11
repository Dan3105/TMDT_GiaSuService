namespace GiaSuService.Models.UtilityViewModel
{
    public class TransactionStatisticsViewModel
    {
        public int TotalTransactions { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalRefund { get; set; }
        public decimal TotalDeposit { get; set; }


        public string JsonTransactionType { get; set; } = string.Empty;
        public string JsonTransactionStatus { get; set; } = string.Empty;
    }

    public class TransactionStatisticByDateViewModel
    {
        public decimal TotalProfit { get; set; }
        public decimal TotalRefund { get; set; }
        public decimal TotalDeposit { get; set; }

        public string JsonTransactionStatus { get; set; } = string.Empty;
        public string JsonTransactionType { get; set; } = string.Empty;
        public string JsonTransaction { get; set; } = string.Empty;
    }
}