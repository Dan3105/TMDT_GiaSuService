using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Models.UtilityViewModel
{
    public class PageAccountListViewModel
    {
    }

    public class PageCardListViewModel
    {
        public List<TutorCardViewModel> list = new List<TutorCardViewModel>();
        public int TotalElement;
    }

    public class PageTutorRequestListViewModel
    {
        public List<TutorRequestCardViewModel> list = new List<TutorRequestCardViewModel>();
        public int TotalElement;
    }

    public class PageTutorRegisterListViewModel
    {
        public List<TutorRegisterViewModel> list = new List<TutorRegisterViewModel>();
        public int TotalElement;
    }

    public class PageTutorRequestQueueListViewModel
    {
        public List<TutorRequestQueueViewModel> list = new List<TutorRequestQueueViewModel>();
        public int TotalElement;
    }


    public class PageTransactionListViewModel
    {
        public List<TransactionCardViewModel> list = new List<TransactionCardViewModel>();
        public int TotalElement;
    }
}
