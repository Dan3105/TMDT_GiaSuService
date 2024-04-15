using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Models.EmployeeViewModel
{
    public class TutorApplyRequestViewModel
    {
        //public TutorRequestCardViewModel tutorRequestInfo { get; set; } = new TutorRequestCardViewModel();
        public IEnumerable<TutorApplyRequestQueueViewModel> tutors { get; set; } = new List<TutorApplyRequestQueueViewModel>();
        public List<string> queriesStatus { get; } = Enum.GetNames(typeof(AppConfig.QueueStatus)).ToList();
    }

    public class TutorApplyRequestQueueViewModel
    {
        public int TutorId { get; set; }    
        public string Avatar { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string TutorType { get; set; } = string.Empty;
        public string StatusQueue { get; set; } = string.Empty;
        public bool IsHaveTransaction { get; set; } = false;
    }

}
