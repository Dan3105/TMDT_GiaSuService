using GiaSuService.Models.EmployeeViewModel;

namespace GiaSuService.Models.TutorViewModel
{
    public class TutorProfileStatusDetailHistoryViewModel
    {
        public int HistoryId { get; set; }
        public string Context { set; get; } = string.Empty;
        public string StatusType { get; set; } = string.Empty;
        public string StatusVNamese { get; set; } = string.Empty;
        public DateTime Date { set; get; }

        public TutorFormUpdateProfileViewModel? DetailModified { set; get; }
        public TutorFormUpdateProfileViewModel? DetailOriginal { set; get; }
    }
}
