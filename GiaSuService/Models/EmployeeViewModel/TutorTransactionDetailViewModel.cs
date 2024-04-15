using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Models.EmployeeViewModel
{
    public class TutorTransactionDetailViewModel
    {
        public TransactionDetailViewModel? TransactionDeposit { get; set; }
        public TransactionDetailViewModel? TransactionRefund { get; set; }

    }
}
