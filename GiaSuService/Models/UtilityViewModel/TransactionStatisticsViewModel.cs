namespace GiaSuService.Models.UtilityViewModel
{
    public class TransactionStatisticsViewModel
    {
        public int TotalTransactions { get; set; }
        public int TotalTransactionsPaid { get; set; }
        public int TotalTransactionsCancel { get; set; }
        public int TotalTransactionsRefund { get; set; }
        public int TotalTransactionsDeposit { get;set; }

        //public decimal TotalTransactionsFee { get; set; }
    }

    

    }