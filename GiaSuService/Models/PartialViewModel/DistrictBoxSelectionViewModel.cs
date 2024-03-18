using GiaSuService.Models.UtilityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.PartialViewModel
{
    public class DistrictBoxSelectionViewModel
    {
        public IEnumerable<DistrictViewModel> Districts { get; set; } = new List<DistrictViewModel>();

        [Required]
        public DistrictViewModel? SelectedDistrict { get; set; }
        public bool IsReadOnly;
    }
}
