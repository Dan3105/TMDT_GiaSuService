using GiaSuService.Models.IdentityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.EmployeeViewModel
{
    public class ContextReviewingRegister
    {
        public TutorProfileViewModel? TutorProfileVM { get; set; }

        [Required]
        public required string Context { get; set; } 

        public string StatusType {  get; set; } = string.Empty;
    }
}
