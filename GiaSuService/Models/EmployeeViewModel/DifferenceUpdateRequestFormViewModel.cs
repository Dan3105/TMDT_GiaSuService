using GiaSuService.EntityModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Models.EmployeeViewModel
{
    public class DifferenceUpdateRequestFormViewModel
    {
        public string Context { set; get;  } = string.Empty;
        public string StatusType { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } 
        public TutorFormUpdateProfileViewModel? Original { get; set; } = null;
        public TutorFormUpdateProfileViewModel? Modified { get; set; } = null;

    }
}
