using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiaSuService.Controllers
{
    [Authorize(Policy = AppConfig.EMPLOYEEPOLICY)]
    public class EmployeeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ITutorRequestFormService _tutorRequestService;
        private readonly IAddressService _addressService;
        private readonly ICatalogService _catalogService;
        private readonly IProfileService _profileService;

        public EmployeeController(ITutorService tutorService, ITutorRequestFormService tutorRequestFormService, IAddressService addressService, ICatalogService catalogService,
            IProfileService profileService)
        {
            _tutorService = tutorService;
            _tutorRequestService = tutorRequestFormService;
            _addressService = addressService;
            _catalogService = catalogService;
            _profileService = profileService;
        }

        public IActionResult Index()
        {
            return Ok();
        }


        [HttpGet]
        public IActionResult TutorRegisterQueue()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorRegisterQueue(int page)
        {
            List<TutorRegisterViewModel> queries = await _tutorService.GetRegisterTutoByStatus(page, AppConfig.RegisterStatus.PENDING);
            int totalPages = (int)Math.Ceiling((double)queries.Count / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> TutorProfileQueue(int id)
        {

            TutorProfileViewModel? tutor = await _tutorService.GetTutorprofileById(id);
            if (tutor == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "User cannot be found";
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }

            if (!tutor.Formstatus.Equals(AppConfig.FormStatus.PENDING.ToString().ToLower()))
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Đơn này đã được xử lý";
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }

            ContextReviewingRegister vm = new ContextReviewingRegister
            {
                TutorProfileVM = tutor,
                Context = string.Empty,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatusTutor(ContextReviewingRegister response)
        {
            if (response.TutorProfileVM == null || response.TutorProfileVM.TutorId == 0)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Error 404";
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }

            var result = await _tutorService.UpdateTutorProfileStatus(response.TutorProfileVM!.TutorId, response.StatusType, response.Context);
            if (result.Success) {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }
            TempData[AppConfig.MESSAGE_FAIL] = result.Message;
            return RedirectToAction("TutorRegisterQueue", "Employee");
        }

        [HttpGet]
        public IActionResult TutorRequestQueue()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorRequestOnPending(int page)
        {
            List<TutorRequestQueueViewModel> queries = await _tutorRequestService.GetTutorrequestQueue(AppConfig.FormStatus.PENDING, page);
            int totalPages = (int)Math.Ceiling((double)queries.Count / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> TutorRequestProfile(int id)
        {
            var thisForm = await _tutorRequestService.GetTutorRequestFormById(id);
            if (thisForm == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Không tìm thấy thông tin đơn vui lòng làm lại";
                return RedirectToAction("TutorRequestQueue", "Employee");
            }
            return View(thisForm);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStatusTutorRequest(int formid, string statusName)
        {
            var result = await _tutorRequestService.UpdateStatusTutorRequest(formid, statusName);
            if (result.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                return RedirectToAction("TutorRequestQueue", "Employee");
            }
            TempData[AppConfig.MESSAGE_FAIL] = result.Message;
            return RedirectToAction("TutorRequestQueue", "Employee", new { id = formid });

        }

        [HttpGet]
        public async Task<IActionResult> TutorList()
        {
            var gradeViews = await _catalogService.GetAllGrades();
            var provinceViews = await _addressService.GetProvinces();
            var subjectViews = await _catalogService.GetAllSubjects();

            TutorCardListViewModel result = new TutorCardListViewModel()
            {
                GradeList = gradeViews,
                ProvinceList = provinceViews,
                SubjectList = subjectViews,
            };

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> TutorProfile(int id)
        {
            TutorProfileViewModel? account = await _tutorService.GetTutorprofileById(id);
            if (account == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã gia sư không tồn tại";
                return RedirectToAction("TutorList", "Employee");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> TutorProfile(TutorProfileViewModel tutorProfileViewModel)
        {
            ResponseService result = await _profileService.UpdateTutorProfileInEmployee(tutorProfileViewModel);

            if (result.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                return RedirectToAction("TutorList", "Employee");
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = result.Message;
                return RedirectToAction("TutorProfile", "Employee", new { id = tutorProfileViewModel.TutorId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TutorRequestList()
        {
            var gradeViews = await _catalogService.GetAllGrades();
            var provinceViews = await _addressService.GetProvinces();
            var subjectViews = await _catalogService.GetAllSubjects();

            TutorRequestListViewModel result = new TutorRequestListViewModel()
            {
                GradeList = gradeViews,
                ProvinceList = provinceViews,
                SubjectList = subjectViews,
            };

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> TutorRequestEditByEmployee(int id)
        {
            var thisForm = await _tutorRequestService.GetTutorRequestProfileEdit(id);
            if (thisForm == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Không tìm thấy thông tin đơn vui lòng làm lại";
                return RedirectToAction("TutorRequestList", "Employee");
            }

            if (thisForm.CurrentStatus.ToLower() != AppConfig.FormStatus.APPROVAL.ToString().ToLower())
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Đơn này đã được giao hoặc chưa được duyệt";
                return RedirectToAction("TutorRequestList", "Employee");
            }
            var gradeViews = await _catalogService.GetAllGrades();
            var sessionViews = await _catalogService.GetAllSessions();
            var subjectViews = await _catalogService.GetAllSubjects();

            thisForm.Sessions = sessionViews;
            thisForm.Grades = gradeViews;
            thisForm.Subjects = subjectViews;

            return View(thisForm);
        }

        [HttpGet]
        public IActionResult TutorUpdateQueue()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorUpdateForms(int page)
        {
            List<TutorRegisterViewModel> queries = await _tutorService.GetRegisterTutoByStatus(page, AppConfig.RegisterStatus.UPDATE);
            int totalPages = (int)Math.Ceiling((double)queries.Count / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> ATutorUpdateForm(int tutorId)
        {
            var viewModel = await _tutorService.GetTutorUpdateRequest(tutorId);
            if(viewModel == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Hệ thống không lấy được thông tin này";
                return RedirectToAction("TutorUpdateQueue", "Employee");
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTutorProfileInUpdateQueue(DifferenceUpdateRequestFormViewModel view)
        {
            ResponseService result = await _tutorService.UpdateTutorProfileStatus(view.Modified.TutorId, view.StatusType, view.Context);
            if (!result.Success)
            {
                TempData[AppConfig.MESSAGE_FAIL] = result.Message;
                return RedirectToAction("ATutorUpdateForm", "Employee", new { id = view.Modified.TutorId });
            }
            TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
            return RedirectToAction("TutorUpdateQueue", "Employee");
        }
    }
}
