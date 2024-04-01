using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GiaSuService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        
        //public HomeController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService)
        //{
        //    _tutorService = tutorService;
        //    _catalogService = catalogService;
        //    _addressService = addressService;
        //}

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = AppConfig.ADMINPOLICY)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> TutorList()
        {
            var tutorList = await _tutorService.GetTutorprofilesByFilter(0, 0, 0);
            List<TutorCardViewModel> tutorCards = new List<TutorCardViewModel>();
            //foreach(var tutor in tutorList)
            //{
            //    tutorCards.Add(new TutorCardViewModel()
            //    {
            //        Id = tutor.Id,
            //        AdditionalProfile = tutor.Additionalinfo ?? "",
            //        Area = tutor.Area,
            //        Avatar = tutor.Account.Avatar,
            //        TeachingArea = string.Join(", ", tutor.Districts.Select(d => d.Districtname)),
            //        Birth = tutor.Account.Birth.ToString("dd/MM/yyyy"),
            //        College = tutor.College,
            //        TutorType = tutor.Currentstatus.ToString(),
            //        FullName = tutor.Account.Fullname,
            //        GradeList = string.Join(", ", tutor.Grades.Select(g => g.Gradename)),
            //        GraduateYear = tutor.Academicyearto,
            //        SubjectList = string.Join(", ", tutor.Subjects.Select(g => g.Subjectname))
            //    });
            //}
            //List<Province> provinceList = await _addressService.GetProvinces();
            //List<Subject> subjectList = await _catalogService.GetAllSubjects();
            //List<Grade> gradeList = await _catalogService.GetAllGrades();

            //List<ProvinceViewModel> provinceViews = Utility.ConvertToProvinceViewList(provinceList);
            //List<SubjectViewModel> subjectViews = Utility.ConvertToSubjectViewList(subjectList);
            //List<GradeViewModel> gradeViews = Utility.ConvertToGradeViewList(gradeList);

            TutorCardListViewModel result = new TutorCardListViewModel() { 
                TutorList = tutorCards,
                //GradeList = gradeViews,
                //ProvinceList = provinceViews,
                //SubjectList = subjectViews,
            };

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorsBy(int districtId, int gradeId, int subjectId)
        {
            var tutorList = await _tutorService.GetTutorprofilesByFilter(subjectId, districtId, gradeId);
            List<TutorCardViewModel> tutorCards = new List<TutorCardViewModel>();
            foreach (var tutor in tutorList)
            {
                //tutorCards.Add(new TutorCardViewModel()
                //{
                //    Id = tutor.Id,
                //    AdditionalProfile = tutor.Additionalinfo ?? "",
                //    Area = tutor.Area,
                //    Avatar = tutor.Account.Avatar,
                //    TeachingArea = string.Join(", ", tutor.Districts.Select(d => d.Districtname)),
                //    Birth = tutor.Account.Birth.ToString("dd/MM/yyyy"),
                //    College = tutor.College,
                //    TutorType = tutor.Currentstatus.ToString(),
                //    FullName = tutor.Account.Fullname,
                //    GradeList = string.Join(", ", tutor.Grades.Select(g => g.Gradename)),
                //    GraduateYear = tutor.Academicyearto,
                //    SubjectList = string.Join(", ", tutor.Subjects.Select(g => g.Subjectname))
                //});
            }
            return Json(tutorCards);
        }
    }
}