﻿using GiaSuService.Configs;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.UtilityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.TutorViewModel
{
    public class TutorUpdateRequestViewModel
    {
        [Required]
        public TutorFormUpdateProfileViewModel Form { get; set; } = new TutorFormUpdateProfileViewModel();
        public List<ProvinceViewModel> ProvinceList { get; set; } = new List<ProvinceViewModel>();
        public List<GradeViewModel> GradeList { get; set; } = new List<GradeViewModel>() { };
        public List<SubjectViewModel> SubjectList { get; set; } = new List<SubjectViewModel>() { };
        public List<SessionViewModel> SessionList { get; set; } = new List<SessionViewModel>() { };
        public List<TutorTypeViewModel> TutorTypeList { get; set; } = new List<TutorTypeViewModel>() { };


        public List<int> DistrictSelected { get; set; } = new List<int>();
        public List<int> GradeSelected => GradeList.Where(p => p.IsChecked).Select(p => p.GradeId).ToList();
        public List<int> SubjectSelected => SubjectList.Where(p => p.IsChecked).Select(p => p.SubjectId).ToList();
        public List<int> SessionSelected => SessionList.Where(p => p.IsChecked).Select(p => p.SessionId).ToList();
    }

    public class TutorFormUpdateProfileViewModel
    {
        public int TutorId { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống họ và tên.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Họ và tên chứa 5-100 ký tự.")]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Họ và tên không được chứa số.")]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng không để trống ngày sinh.")]
        public DateOnly? Birth { get; set; } = null;

        [Required(ErrorMessage = "Vui lòng không để trống email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng không để trống số điện thoại.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại chỉ bao gồm 10 chữ số.")]
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng không để trống số nhà, tên đường.")]
        [StringLength(255, ErrorMessage = "Số nhà, tên đường chứa không quá 255 ký tự.")]
        public string AddressDetail { get; set; } = string.Empty;
        public int SelectedProvinceId { get; set; }
        public int SelectedDistrictId { get; set; }


        [Required(ErrorMessage = "Vui lòng không để trống CMND/CCCD.")]
        [RegularExpression(@"^\d{9}$|^\d{12}$", ErrorMessage = "CMND chỉ gồm 9 chữ số hoặc CCCD chỉ gồm 12 chữ số.")]
        public string IdentityCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng tải ảnh mặt trước CMND/CCCD.")]
        public string FrontIdentityCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng tải ảnh mặt sau CMND/CCCD.")]
        public string BackIdentityCard { get; set; } = string.Empty;


        [Required(ErrorMessage = "Vui lòng nhập tên trường")]
        public string College { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập chuyên ngành")]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Chuyên ngành không được chứa số.")]
        public string Area { get; set; } = string.Empty;
        public string? Additionalinfo { get; set; }
        public bool IsActive { get; set; }
        public string TutorStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập năm bắt đầu")]
        public short Academicyearfrom { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập năm kết thúc")]
        public short Academicyearto { get; set; }
        public int SelectedTutorTypeId { get; set; }

        public string FormatAddress { get; set; } = string.Empty;
        public string FormatTutorType { get; set; } = string.Empty;
        public string FormatTeachingArea { get; set; } = string.Empty;
        public string FormatGrades { get; set; } = string.Empty;
        public string FormatSessions { get; set; } = string.Empty;
        public string FormatSubjects { get; set; } = string.Empty;

        public IEnumerable<int> SelectedSubjectIds { get; set; } = new List<int>();
        public IEnumerable<int> SelectedSessionIds { get; set; } = new List<int>();
        public IEnumerable<int> SelectedDistricts { get; set; } = new List<int>();
        public IEnumerable<int> SelectedGradeIds { get; set; } = new List<int>();

        public static TutorFormUpdateProfileViewModel? CompareDifference(TutorFormUpdateProfileViewModel origin, TutorFormUpdateProfileViewModel modified)
        {
            TutorFormUpdateProfileViewModel diff = new TutorFormUpdateProfileViewModel();
            if(origin.TutorId != modified.TutorId)
            {
                return null;
            }
            diff.TutorId = origin.TutorId;
            diff.IsActive = modified.IsActive;
            if (origin.Fullname != Utility.FormatToCamelCase(modified.Fullname))
                diff.Fullname = Utility.FormatToCamelCase(modified.Fullname);

            if (origin.Birth != modified.Birth)
                diff.Birth = modified.Birth;

            if (origin.Email != modified.Email)
                diff.Email = modified.Email;

            if (origin.Phone != modified.Phone)
                diff.Phone = modified.Phone;

            if (origin.Gender != modified.Gender)
                diff.Gender = modified.Gender;

            if (origin.Avatar != modified.Avatar)
                diff.Avatar = modified.Avatar;

            if (origin.AddressDetail != modified.AddressDetail)
                diff.AddressDetail = modified.AddressDetail;

            if (origin.SelectedDistrictId != modified.SelectedDistrictId)
                diff.SelectedDistrictId = modified.SelectedDistrictId;

            if (origin.IdentityCard != modified.IdentityCard)
                diff.IdentityCard = modified.IdentityCard;

            if (origin.FrontIdentityCard != modified.FrontIdentityCard)
                diff.FrontIdentityCard = modified.FrontIdentityCard;

            if (origin.BackIdentityCard != modified.BackIdentityCard)
                diff.BackIdentityCard = modified.BackIdentityCard;

            if (origin.College != Utility.FormatToCamelCase(modified.College))
                diff.College = Utility.FormatToCamelCase(modified.College);

            if (origin.Area != Utility.FormatToCamelCase(modified.Area))
                diff.Area = Utility.FormatToCamelCase(modified.Area);

            if (origin.Additionalinfo != modified.Additionalinfo)
                diff.Additionalinfo = modified.Additionalinfo;

            if (origin.Academicyearfrom != modified.Academicyearfrom)
                diff.Academicyearfrom = modified.Academicyearfrom;

            if (origin.Academicyearto != modified.Academicyearto)
                diff.Academicyearto = modified.Academicyearto;

            if (origin.SelectedTutorTypeId != modified.SelectedTutorTypeId)
                diff.SelectedTutorTypeId = modified.SelectedTutorTypeId;

            if (!AreListsEqual(origin.SelectedSubjectIds, modified.SelectedSubjectIds))
                diff.SelectedSubjectIds = modified.SelectedSubjectIds;

            if (!AreListsEqual(origin.SelectedSessionIds, modified.SelectedSessionIds))
                diff.SelectedSessionIds = modified.SelectedSessionIds;

            if (!AreListsEqual(origin.SelectedDistricts, modified.SelectedDistricts))
                diff.SelectedDistricts = modified.SelectedDistricts;

            // Check if any property is different, if yes, return the diff object, else return null
            return IsAnyPropertyDifferent(diff) ? diff : null;
        }

        private static bool AreListsEqual(IEnumerable<int> list1, IEnumerable<int> list2)
        {
            // Check if there are any elements in list1 that are not in list2
            var diff1 = list1.Except(list2);

            // Check if there are any elements in list2 that are not in list1
            var diff2 = list2.Except(list1);

            // If both differences are empty, lists are equal
            return !diff1.Any() && !diff2.Any();
        }

        private static bool IsAnyPropertyDifferent(TutorFormUpdateProfileViewModel viewModel)
        {
            // Check for all properties, except collections, since they are reference types
            return
                   viewModel.Fullname != string.Empty ||
                   viewModel.Email != string.Empty ||
                   viewModel.Phone != string.Empty ||
                   viewModel.Gender != string.Empty ||
                   viewModel.Birth != null ||
                   viewModel.Avatar != string.Empty ||
                   viewModel.AddressDetail != string.Empty ||
                   viewModel.IdentityCard != string.Empty ||
                   viewModel.FrontIdentityCard != string.Empty ||
                   viewModel.BackIdentityCard != string.Empty ||
                   viewModel.College != string.Empty ||
                   viewModel.Area != string.Empty ||
                   viewModel.Additionalinfo != null ||
                   //viewModel.IsActive != false ||
                   viewModel.SelectedDistrictId != 0 ||
                   viewModel.Academicyearfrom != 0 ||
                   viewModel.Academicyearto != 0 ||
                   viewModel.SelectedTutorTypeId != 0 ||
                   viewModel.SelectedSubjectIds.Any() ||
                   viewModel.SelectedDistricts.Any() ||
                   viewModel.SelectedGradeIds.Any() ||
                   viewModel.SelectedSessionIds.Any();
        }
    }

}
