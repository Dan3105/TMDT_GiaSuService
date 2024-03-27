using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace GiaSuService.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly ITutorService _tutorService;
        private readonly ITutorRequestFormService _tutorRequestFormService;
        
        
        public CustomerController(ICatalogService catalogService, IAddressService addressService, ITutorService tutorService
            ,ITutorRequestFormService tutorRequestFormService)
        {
            _addressService = addressService;
            _catalogService = catalogService;
            _tutorService = tutorService;
            _tutorRequestFormService = tutorRequestFormService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = AppConfig.CUSTOMERPOLICY)]
        public async Task<IActionResult> TutorRequestForm(int? tutorId = null)
        {
            var provinces = await _addressService.GetProvinces();
            var grades = await _catalogService.GetAllGrades();
            var subjects = await _catalogService.GetAllSubjects();

            FormTutorRequestViewModel vm = new FormTutorRequestViewModel()
            {
                Profile = new TutorRequestProfile(),
                Grades = Utility.ConvertToGradeViewList(grades),
                Subjects = Utility.ConvertToSubjectViewList(subjects),
                Provinces = Utility.ConvertToProvinceViewList(provinces),
                TutorCards = new List<TutorCardViewModel>()
            };

            if(tutorId != null)
            {
                AddTutorSelected((int)tutorId);
            }

            var tutorSelectedCookie = GetTutorsSelected();
            
            if (tutorSelectedCookie != null)
            {
                if(tutorId != null)
                {
                    tutorSelectedCookie.Add((int)tutorId);
                }
                List<Tutorprofile> tutorprofiles = await _tutorService.GetSubTutors(tutorSelectedCookie);
                foreach (var profile in tutorprofiles)
                {
                    vm.TutorCards.Add(new TutorCardViewModel()
                    {
                        Id = profile.Id,
                        Avatar = profile.Account.Avatar,
                        FullName = profile.Account.Fullname,
                        Area = profile.Area,
                        College = profile.College,
                        TutorType = profile.Currentstatus.ToString()
                    });
                }
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult DeleteTutorRequest(int id)
        {
            RemoveTutorSelected(id);
            return RedirectToAction("TutorRequestForm", "Customer");
        }

        private List<int>? GetTutorsSelected()
        {
            var cookie = Request.Cookies[AppConfig.TUTOR_SELECTED_COOKIE];
            if (cookie != null)
            {
                try
                {
                    List<int>? tutorIds = JsonConvert.DeserializeObject<List<int>>(cookie);
                    return tutorIds;
                }
                catch { }
            }
            return null;
        }

        private void AddTutorSelected(int id)
        {
            var cookie = Request.Cookies[AppConfig.TUTOR_SELECTED_COOKIE];
            if (cookie != null)
            {
                try
                {
                    List<int>? tutorIds = JsonConvert.DeserializeObject<List<int>>(cookie);
                    if(tutorIds != null)
                    {
                        if (tutorIds.Contains(id))
                        {
                            return;
                        }
                        tutorIds.Add(id);
                        SaveListToCookie(tutorIds);
                    }

                }
                catch { }
            }
            else
            {
                List<int> tutors = new List<int> { id };
                SaveListToCookie(tutors);
            }
        }

        private void RemoveTutorSelected(int id)
        {
            var cookie = Request.Cookies[AppConfig.TUTOR_SELECTED_COOKIE];
            if (cookie != null)
            {
                try
                {
                    List<int>? tutorIds = JsonConvert.DeserializeObject<List<int>>(cookie);
                    if (tutorIds != null)
                    {
                        tutorIds.Remove(id);
                        SaveListToCookie(tutorIds);
                    }

                }
                catch { }
            }
        }

        private void ClearTutorSelected()
        {
            Response.Cookies.Delete(AppConfig.TUTOR_SELECTED_COOKIE);
        }

        private void SaveListToCookie(List<int> list)
        {

            var serializedData = JsonConvert.SerializeObject(list);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(20),
                HttpOnly = true
            };

            Response.Cookies.Append(AppConfig.TUTOR_SELECTED_COOKIE, serializedData, cookieOptions);
        }

        [HttpPost]
        [Authorize(Policy = AppConfig.CUSTOMERPOLICY)]
        public async Task<IActionResult> SubmitTutorRequest(FormTutorRequestViewModel req)
        {
            if (!ModelState.IsValid)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Thông tin còn thiếu";
                return RedirectToAction("TutorRequestForm", "Customer");
            }

            int requester = -1;
            int.TryParse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)!.Value, out requester);
            if(requester == -1)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Hết hạn đăng nhập";
                return RedirectToAction("Index", "Identity");
            }
            Tutorrequestform form = new Tutorrequestform()
            {
                Additionaldetail = req.Profile.AdditionalDetail,
                Addressdetail = req.Profile.Addressdetail,
                Createddate = DateTime.Now,
                Expireddate = DateTime.Now.AddDays(30),
                Districtid = req.Profile.DistrictId,
                Gradeid = req.Profile.GradeId,
                Subjectid = req.Profile.SubjectId,
                Nsessions = (short)req.Profile.NSessions,
                Nstudents = (short)req.Profile.NStudents,
                Status = AppConfig.TutorRequestStatus.PENDING,
                Accountid = requester
            };

            List<int>? ids = GetTutorsSelected();
            if(ids != null)
            {
                var listTutors = await _tutorService.GetSubTutors(ids);
                foreach(var tutor in listTutors)
                {
                    form.Tutormatchrequestqueues.Add(new Tutormatchrequestqueue()
                    {
                        Applydate = DateOnly.FromDateTime(DateTime.Now),
                        Tutor = tutor
                    });
                }
            }

            bool isSuccess = await _tutorRequestFormService.CreateForm(form);
            if(isSuccess)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Đã gửi đơn thành công";
                ClearTutorSelected();
                return RedirectToAction("Index", "Home");
            }

            TempData[AppConfig.MESSAGE_FAIL] = "Lỗi hệ thống vui lòng làm lại đi hihi";
            return RedirectToAction("TutorRequestForm", "Customer");
        }
    }
}
