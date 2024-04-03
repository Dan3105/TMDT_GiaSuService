﻿using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Models.EmployeeViewModel
{
    public class TutorRequestProfileViewModel
    {
        public int FormId { get; set; }
        public string FullNameRequester { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string AddressName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;

        public List<TutorCardViewModel> TutorCards { get; set; } = new List<TutorCardViewModel>();

    }
}
