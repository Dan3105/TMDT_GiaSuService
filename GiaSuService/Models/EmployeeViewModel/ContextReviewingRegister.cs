using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.EmployeeViewModel
{
    public class ContextReviewingRegister
    {
        [Required]
        public required string Context { get; set; } 
    }
}
