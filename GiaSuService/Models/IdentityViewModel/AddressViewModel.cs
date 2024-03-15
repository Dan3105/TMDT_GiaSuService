using GiaSuService.EntityModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.IdentityViewModel
{
    public class AddressViewModel
    {
        public DistrictViewModel? District { get; set; }
        public required string AddressName { get; set; }

        public AddressViewModel() { }
    }

    public class DistrictViewModel
    {
        [Required]
        public int DistrictId { get; set; }

        public string DistrictName { get; set; } = "";
        
        [Required]
        public int? ProvinceId;

    }

    public class ProvinceViewModel
    {
        public List<DistrictViewModel> Districts { get; set; } = null!;
        
        [Required]
        public int ProvinceId { get; set; }
        
        [Required]
        public required string ProvinceName { get; set; }
    }
}
