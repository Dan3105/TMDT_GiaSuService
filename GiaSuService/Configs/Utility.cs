using GiaSuService.EntityModel;
using GiaSuService.Models;
using GiaSuService.Models.UtilityViewModel;
using Newtonsoft.Json;

namespace GiaSuService.Configs
{
    public static class Utility
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        //Conversion task
        public static List<GradeViewModel> ConvertToGradeViewList(List<Grade> listGrade)
        {
            var listGradeView = new List<GradeViewModel>();
            foreach (var grade in listGrade)
            {
                listGradeView.Add(new GradeViewModel()
                {
                    GradeName = grade.Gradename,
                    GradeId = grade.Id,
                    IsChecked = false
                });
            }

            return listGradeView;
        }

        public static List<DistrictViewModel> ConvertToDistrictViewList(List<District> listDistrict)
        {
            var listDistrictView = new List<DistrictViewModel>();
            foreach (var district in listDistrict)
            {
                listDistrictView.Add(new DistrictViewModel()
                {
                    DistrictName = district.Districtname,
                    DistrictId = district.Id,
                    IsChecked = false
                });
            }

            return listDistrictView;
        }

        public static List<SubjectViewModel> ConvertToSubjectViewList(List<Subject> listSubject)
        {
            var listSubjectView = new List<SubjectViewModel>();
            foreach (var subject in listSubject)
            {
                listSubjectView.Add(new SubjectViewModel()
                {
                    SubjectName = subject.Subjectname,
                    SubjectId = subject.Id,
                    IsChecked = false
                });
            }

            return listSubjectView;
        }

        public static List<SessionViewModel> ConvertToSessionViewList(List<Sessiondate> listSession)
        {
            var listSessionView = new List<SessionViewModel>();
            foreach (var session in listSession)
            {
                listSessionView.Add(new SessionViewModel()
                {
                    SessionName = session.Sessiondate1,
                    SessionId = session.Id,
                    IsChecked = false
                });
            }

            return listSessionView;
        }


        public static List<ProvinceViewModel> ConvertToProvinceViewList(List<Province> provinces)
        {
            List<ProvinceViewModel> result = new List<ProvinceViewModel>();
            foreach (Province province in provinces)
            {
                result.Add(new ProvinceViewModel
                {
                    ProvinceId = province.Id,
                    ProvinceName = province.Provincename,
                });
            }

            return result;
        }

    }
}
