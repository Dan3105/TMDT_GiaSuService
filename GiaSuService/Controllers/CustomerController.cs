using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Services;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace GiaSuService.Controllers
{
    [Authorize(Policy = AppConfig.CUSTOMERPOLICY)]
    public class CustomerController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly ITutorService _tutorService;
        private readonly IProfileService _profileService;   
        private readonly ITutorRequestFormService _tutorRequestFormService;


        public CustomerController(ICatalogService catalogService, IAddressService addressService, ITutorService tutorService
            , IProfileService profileService, ITutorRequestFormService tutorRequestFormService)
        {
            _addressService = addressService;
            _catalogService = catalogService;
            _tutorService = tutorService;
            _profileService = profileService;
            _tutorRequestFormService = tutorRequestFormService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TutorRequestForm(int? tutorId = null)
        {
            var provinces = await _addressService.GetProvinces();
            var grades = await _catalogService.GetAllGrades();
            var subjects = await _catalogService.GetAllSubjects();
            var sessions = await _catalogService.GetAllSessions();

            FormTutorRequestViewModel vm = new FormTutorRequestViewModel()
            {
                Profile = new TutorRequestProfile(),
                Grades = grades,
                Subjects = subjects,
                Provinces = provinces,
                Sessions= sessions,
                
            };

            if(tutorId != null)
            {
                AddTutorSelected((int)tutorId);
            }

           
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorsSelectedInCookie()
        {
            var tutorCards = new List<TutorCardViewModel>();
            var tutorSelectedCookie = GetTutorsSelected();
            if(tutorSelectedCookie == null)
            {
                return Ok(tutorCards);
            }
            tutorCards = await _tutorService.GetSubTutors(tutorSelectedCookie);

            return Ok(tutorCards);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerListTutorRequest()
        {
            int requester = -1;
            int.TryParse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)!.Value, out requester);
            if (requester == -1)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Hết hạn đăng nhập";
                return RedirectToAction("Index", "Identity");
            }

            int? customerId = await _profileService.GetProfileId(requester, AppConfig.CUSTOMERROLENAME);
            if (customerId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Hết hạn đăng nhập";
                return RedirectToAction("Index", "Identity");
            }

            var listProfile = await _tutorRequestFormService.GetCustomerTutorRequest((int)customerId);
            return View(listProfile);
        }

        [HttpGet]
        public IActionResult DeleteTutorRequest(int id)
        {
            RemoveTutorSelected(id);
            return Ok();
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

            int? customerId = await _profileService.GetProfileId(requester, AppConfig.CUSTOMERROLENAME);
            if(customerId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Hết hạn đăng nhập";
                return RedirectToAction("Index", "Identity");
            }

            RequestTutorForm form = new RequestTutorForm()
            {
                AdditionalDetail = req.Profile.AdditionalDetail,
                AddressDetail = req.Profile.Addressdetail,
                CreateDate = DateTime.Now,
                ExpiredDate = DateTime.Now.AddDays(30),
                DistrictId = req.Profile.DistrictId,
                GradeId = req.Profile.GradeId,
                SubjectId = req.Profile.SubjectId,
                Students = req.Profile.NStudents,
                CustomerId = (int)customerId,
            };

            List<int> ids = GetTutorsSelected() ?? new List<int>();

            ResponseService result = await _tutorRequestFormService.CreateForm(form, req.SessionSelected, ids);
            //bool isSuccess = false;
            if(result.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                ClearTutorSelected();
                return RedirectToAction("Index", "Home");
            }

            TempData[AppConfig.MESSAGE_FAIL] = result.Message;
            return RedirectToAction("TutorRequestForm", "Customer");
        }
    }
}
