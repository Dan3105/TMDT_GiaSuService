using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        private readonly ITransactionService _transactionService;

        public EmployeeController(ITutorService tutorService, ITutorRequestFormService tutorRequestFormService, IAddressService addressService, ICatalogService catalogService,
            IProfileService profileService, ITransactionService transactionService)
        {
            _tutorService = tutorService;
            _tutorRequestService = tutorRequestFormService;
            _addressService = addressService;
            _catalogService = catalogService;
            _profileService = profileService;
            _transactionService = transactionService;
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
            var queries = await _tutorService.GetRegisterTutoByStatus(page, AppConfig.RegisterStatus.PENDING);
            int totalPages = (int)Math.Ceiling((double)queries.TotalElement / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries=queries.list, page, totalPages };
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

            if (result.Success)
            {
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
            var queries = await _tutorRequestService.GetTutorrequestQueue(AppConfig.FormStatus.PENDING, page);
            int totalPages = (int)Math.Ceiling((double)queries.TotalElement / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries=queries.list, page, totalPages };
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

        [HttpPost]
        public async Task<IActionResult> UpdateTutorRequestEdit(TutorRequestProfileEditViewModel model)
        {
            if (model == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Đơn đăng ký tìm gia sư không được rỗng";
                return RedirectToAction("TutorRequestList", "Employee");
            }

            ResponseService response = await _tutorRequestService.UpdateTutorRequestProfileEdit(model);

            if (response.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = response.Message;
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = response.Message;
            }

            return RedirectToAction("TutorRequestEditByEmployee", "Employee", new { id = model.RequestId });
        }

        public async Task<IActionResult> CancelTutorRequest(TutorRequestProfileEditViewModel model)
        {
            if (model == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Đơn đăng ký tìm gia sư không được rỗng";
                return RedirectToAction("TutorRequestList", "Employee");
            }

            ResponseService response = await _tutorRequestService.UpdateStatusTutorRequest(model.RequestId, AppConfig.FormStatus.CANCEL.ToString().ToLower());

            if (response.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Đã huỷ đơn thành công";
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = response.Message;
            }

            return RedirectToAction("TutorRequestEditByEmployee", "Employee", new { id = model.RequestId });
        }

        [HttpGet]
        public IActionResult TutorUpdateQueue()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorUpdateForms(int page)
        {
            PageTutorRegisterListViewModel queries = await _tutorService.GetRegisterTutoByStatus(page, AppConfig.RegisterStatus.UPDATE);
            int totalPages = (int)Math.Ceiling((double)queries.TotalElement / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries=queries.list, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> ATutorUpdateForm(int tutorId)
        {
            var viewModel = await _tutorService.GetTutorUpdateRequest(tutorId);
            if (viewModel == null)
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

        [HttpGet]
        public async Task<IActionResult> TutorApplyQueue(int requestId)
        {
            TutorRequestCardViewModel? tutorCardInfo = await _tutorRequestService.GetTutorrequestDetail(requestId);
            if (tutorCardInfo == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Không tìm được đơn này trong hệ thống";
                return RedirectToAction("TutorRequestList", "Employee");
            }


            return View(tutorCardInfo);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorsOnQueue(int requestId)
        {
            IEnumerable<TutorApplyRequestQueueViewModel> queries = await _tutorRequestService.GetTutorsApplyRequestQueue(requestId);
            List<StatusNamePair> statusList = await _catalogService.GetAllStatus(AppConfig.queue_status);
            TutorApplyRequestViewModel data = new TutorApplyRequestViewModel()
            {
                tutors = queries,
                queriesStatus = statusList
            };
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTutorStatus(int tutorId, int requestId, string newStatus)
        {
            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (accountId == null || accountId == "")
                return Ok(new ResponseService { Message = "Vui lòng đăng nhập lại để thực hiện thao tác", Success = false });

            int? profileId = await _profileService.GetProfileId(int.Parse(accountId), AppConfig.EMPLOYEEROLENAME);

            if (profileId == null)
            {

                return Ok(new ResponseService { Message = "Bạn không phải nhân viên để thực hiện thao tác này", Success = false });
            }

            ResponseService response = await _tutorRequestService.UpdateTutorQueue(requestId, tutorId, newStatus, (int)profileId);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> TutorTransactionDetail(int tutorId, int requestId)
        {
            var result = await _transactionService.GetDetailTutorQueueTransaction(tutorId, requestId);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateRefundTransaction(int tutorId, int requestId)
        {
            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (accountId == null || accountId == "")
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Vui lòng đăng nhập lại để thực hiện thao tác";
                return RedirectToAction("");
            }

            int? empId = await _profileService.GetProfileId(int.Parse(accountId), AppConfig.EMPLOYEEROLENAME);

            if (empId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Bạn không phải nhân viên để thực hiện thao tác này";
                return RedirectToAction("");
            }

            var response = await _transactionService.CreateRefundTransaction(tutorId, requestId, (int)empId);
            if (response.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = response.Message;
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = response.Message;
            }
            return RedirectToAction("TutorTransactionDetail", "Employee", new { tutorId, requestId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDepositTransaction(int tutorId, int requestId, DateTime paymentDate)
        {
            var response = await _transactionService.UpdateDepositPaymentTransaction(tutorId, requestId, paymentDate);
            if (response.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = response.Message;
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = response.Message;
            }
            return RedirectToAction("TutorTransactionDetail", "Employee", new { tutorId, requestId });
        }
    }
}
