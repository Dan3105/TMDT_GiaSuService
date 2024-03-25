using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.PartialViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.UtilityViewModel
{
    public class RegisterFormViewModel
    {
        public RegisterAccountProfileViewModel? RegisterForm { get; set; }
        public List<ProvinceViewModel> ProvinceList { get; set; } = new List<ProvinceViewModel>();
        public RegisterFormViewModel() { }
        public bool ConfirmBox { get; set; }
    }

}
