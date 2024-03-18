using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.PartialViewModel;
using GiaSuService.Models.UtilityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.AdminViewModel
{
    public class RegisterEmployeeViewModel
    {
        public RegisterAccountProfileViewModel? RegisterForm { get; set; }
        public List<ProvinceViewModel> ProvinceList { get; set;} = new List<ProvinceViewModel>();
        public RegisterEmployeeViewModel () { }
    }

}
