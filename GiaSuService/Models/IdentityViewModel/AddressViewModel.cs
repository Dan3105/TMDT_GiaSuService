using GiaSuService.Models.UtilityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.IdentityViewModel
{
    public class AddressViewModel
    {
        public DistrictViewModel? District { get; set; }
       
        public AddressViewModel() { }
    }
}
