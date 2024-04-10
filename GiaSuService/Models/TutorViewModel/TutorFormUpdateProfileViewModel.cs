﻿using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Models.TutorViewModel
{
    public class TutorUpdateRequestViewModel
    {
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
        public string Fullname { get; set; } = string.Empty;
        public DateOnly Birth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        public string AddressDetail { get; set; } = string.Empty;
        public int SelectedDistrictId { get; set; }
        public int SelectedProvinceId { get; set; }

        public string IdentityCard { get; set; } = string.Empty;
        public string FrontIdentityCard { get; set; } = string.Empty;
        public string BackIdentityCard { get; set; } = string.Empty;

        public string College { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Additionalinfo { get; set; }
        public short Academicyearfrom { get; set; }
        public short Academicyearto { get; set; }
        public int SelectedTutorTypeId { get; set; }

        public IEnumerable<int> selectedSubjectIds { get; set; } = new List<int>();
        public IEnumerable<int> selectedSessionIds { get; set; } = new List<int>();
        public IEnumerable<int> selectedDistricts { get; set; } = new List<int>();
        public IEnumerable<int> selectedGradeIds { get; set; } = new List<int>();

        public static TutorFormUpdateProfileViewModel? CompareDifference(TutorFormUpdateProfileViewModel origin, TutorFormUpdateProfileViewModel modified)
        {
            TutorFormUpdateProfileViewModel diff = new TutorFormUpdateProfileViewModel();
            if(origin.TutorId != modified.TutorId)
            {
                return null;
            }
            diff.TutorId = origin.TutorId;
            diff.IsActive = modified.IsActive;
            if (origin.Fullname != modified.Fullname)
                diff.Fullname = modified.Fullname;

            if (origin.Birth != modified.Birth)
                diff.Birth = modified.Birth;

            if (origin.Email != modified.Email)
                diff.Email = modified.Email;

            if (origin.Phone != modified.Phone)
                diff.Phone = modified.Phone;

            if (origin.Gender != modified.Gender)
                diff.Gender = modified.Gender;

            //if (origin.Avatar != modified.Avatar)
            //    diff.Avatar = modified.Avatar;

            if (origin.AddressDetail != modified.AddressDetail)
                diff.AddressDetail = modified.AddressDetail;

            if (origin.SelectedDistrictId != modified.SelectedDistrictId)
                diff.SelectedDistrictId = modified.SelectedDistrictId;

            if (origin.SelectedProvinceId != modified.SelectedProvinceId)
                diff.SelectedProvinceId = modified.SelectedProvinceId;

            if (origin.IdentityCard != modified.IdentityCard)
                diff.IdentityCard = modified.IdentityCard;

            //if (origin.FrontIdentityCard != modified.FrontIdentityCard)
            //    diff.FrontIdentityCard = modified.FrontIdentityCard;

            //if (origin.BackIdentityCard != modified.BackIdentityCard)
            //    diff.BackIdentityCard = modified.BackIdentityCard;

            if (origin.College != modified.College)
                diff.College = modified.College;

            if (origin.Area != modified.Area)
                diff.Area = modified.Area;

            if (origin.Additionalinfo != modified.Additionalinfo)
                diff.Additionalinfo = modified.Additionalinfo;

            if (origin.Academicyearfrom != modified.Academicyearfrom)
                diff.Academicyearfrom = modified.Academicyearfrom;

            if (origin.Academicyearto != modified.Academicyearto)
                diff.Academicyearto = modified.Academicyearto;

            if (origin.SelectedTutorTypeId != modified.SelectedTutorTypeId)
                diff.SelectedTutorTypeId = modified.SelectedTutorTypeId;

            if (!origin.selectedSubjectIds.SequenceEqual(modified.selectedSubjectIds))
                diff.selectedSubjectIds = modified.selectedSubjectIds;

            if (!origin.selectedSessionIds.SequenceEqual(modified.selectedSessionIds))
                diff.selectedSessionIds = modified.selectedSessionIds;

            if (!origin.selectedDistricts.SequenceEqual(modified.selectedDistricts))
                diff.selectedDistricts = modified.selectedDistricts;

            if (!origin.selectedGradeIds.SequenceEqual(modified.selectedGradeIds))
                diff.selectedGradeIds = modified.selectedGradeIds;

            // Check if any property is different, if yes, return the diff object, else return null
            return IsAnyPropertyDifferent(diff) ? diff : null;
        }

        private static bool IsAnyPropertyDifferent(TutorFormUpdateProfileViewModel viewModel)
        {
            // Check for all properties, except collections, since they are reference types
            return 
                   viewModel.Fullname != string.Empty ||
                   viewModel.Email != string.Empty ||
                   viewModel.Phone != string.Empty ||
                   viewModel.Gender != string.Empty ||
                   //viewModel.Avatar != string.Empty ||
                   viewModel.AddressDetail != string.Empty ||
                   viewModel.IdentityCard != string.Empty ||
                   //viewModel.FrontIdentityCard != string.Empty ||
                   //viewModel.BackIdentityCard != string.Empty ||
                   viewModel.College != string.Empty ||
                   viewModel.Area != string.Empty ||
                   viewModel.Additionalinfo != null ||
                   //viewModel.IsActive != false ||
                   viewModel.Academicyearfrom != 0 ||
                   viewModel.Academicyearto != 0 ||
                   viewModel.SelectedTutorTypeId != 0;
        }
    }

}
