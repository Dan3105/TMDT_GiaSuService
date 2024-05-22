using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
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

        private async Task<ProfileViewModel?> GetCurrentCustomer()
        {
            var userRole = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value;

            if (userRole == AppConfig.TUTORROLENAME) return null;

            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            //User not founded
            if (accountId == null || accountId == "" || userRole == null) return null;

            var profileId = await _profileService.GetProfileId(int.Parse(accountId), userRole);

            if (profileId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return null;
            }

            var profile = await _profileService.GetProfile((int)profileId, userRole);
            if (profile == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return null;
            }

            return profile;
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

            var currProfile = await GetCurrentCustomer();

            vm.Profile.Addressdetail = currProfile == null ? "" : currProfile.AddressDetail;
            vm.Profile.SelectedProvinceId = currProfile == null ? -1 : currProfile.SelectedProvinceId;
            vm.Profile.DistrictId = currProfile == null ? -1 : currProfile.SelectedDistrictId;

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

        [HttpGet]
        public async Task<IActionResult> TutorRequestEdit(int id)
        {
            var thisForm = await _tutorRequestFormService.GetTutorRequestProfileEdit(id);
            if (thisForm == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Không tìm thấy thông tin đơn vui lòng làm lại";
                return RedirectToAction("CustomerListTutorRequest", "Customer");
            }

            var gradeViews = await _catalogService.GetAllGrades();
            var sessionViews = await _catalogService.GetAllSessions();
            var subjectViews = await _catalogService.GetAllSubjects();

            thisForm.Sessions = sessionViews;
            thisForm.Grades = gradeViews;
            thisForm.Subjects = subjectViews;

            return View(thisForm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTutorRequestEdit(TutorRequestProfileEditViewModel model)
        {
            if (model == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Đơn đăng ký tìm gia sư không được rỗng";
                return RedirectToAction("CustomerListTutorRequest", "Customer");
            }

            ResponseService response = await _tutorRequestFormService.UpdateTutorRequestProfileEdit(model);

            if (response.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = response.Message;
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = response.Message;
            }

            return RedirectToAction("TutorRequestEdit", "Customer", new { id = model.RequestId });
        }

        public async Task<IActionResult> CancelTutorRequest(TutorRequestProfileEditViewModel model)
        {
            if (model == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Đơn đăng ký tìm gia sư không được rỗng";
                return RedirectToAction("CustomerListTutorRequest", "Customer");
            }

            ResponseService response = await _tutorRequestFormService.UpdateStatusTutorRequest(model.RequestId, AppConfig.FormStatus.CANCEL.ToString().ToLower());

            if (response.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Đã huỷ đơn thành công";
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = response.Message;
            }

            return RedirectToAction("TutorRequestEdit", "Customer", new { id = model.RequestId });
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorsOnQueue(int requestId)
        {
            IEnumerable<TutorApplyRequestQueueViewModel> queries = await _tutorRequestFormService.GetTutorsApplyRequestQueue(requestId);
            List<StatusNamePair> statusList = await _catalogService.GetAllStatus(AppConfig.queue_status);
            TutorApplyRequestViewModel data = new TutorApplyRequestViewModel()
            {
                tutors = queries,
                queriesStatus = statusList
            };
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> TutorProfile(int tutorId, int requestId)
        {
            TutorProfileViewModel? account = await _tutorService.GetTutorprofileById(tutorId);
            if (account == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã gia sư không tồn tại";
                return RedirectToAction("TutorList", "Employee");
            }
            TempData["RequestId"] = requestId;

            return View(account);
        }
    }
}
