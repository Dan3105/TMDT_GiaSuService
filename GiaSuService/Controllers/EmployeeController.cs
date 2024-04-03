using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Services;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            List<TutorRegisterViewModel> queries = await _tutorService.GetRegisterTutorOnPending(page);
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
            return View(tutor);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStatusTutor(int id, string statusType)
        {
            var result = await _tutorService.UpdateTutorProfileStatus(id, statusType);
            if(result.Success) {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }
            TempData[AppConfig.MESSAGE_FAIL] = result.Message;
            return RedirectToAction("TutorRegisterQueue", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> TutorRequestQueue()
        {
            //var listRequest = await _tutorRequestService.GetTutorrequestforms(AppConfig.TutorRequestStatus.PENDING);
            
            List<TutorRequestItemViewModel> listRequest = new List<TutorRequestItemViewModel>();
            List<TutorRequestItemViewModel> results = new List<TutorRequestItemViewModel>();
            foreach (var request in listRequest)
            {
                //string GradeName = (await _catalogService.GetGradeById(request.Gradeid))?.Gradename ?? "";
                //var SubjectName = (await _catalogService.GetSubjectById(request.Subjectid))?.Subjectname ?? "";
                //var address = await _addressService.GetDistrictData(request.Districtid);
                //string AddressName = $"{address.Province.Provincename}, {address.Districtname}, {request.Addressdetail}";
                //results.Add(new TutorRequestItemViewModel
                //{ 
                //    FormId = request.Id,
                //    FullNameRequester = request.Account.Fullname,
                //    AddressName = AddressName,
                //    CreatedDate = DateOnly.FromDateTime(request.Createddate),
                //    GradeName = GradeName,
                //    SubjectName = SubjectName,
                //});
            }
            return View(results);
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
            //var tutorInQueue = await _tutorService.GetTutorprofilesByClassId(id);

            //string GradeName = (await _catalogService.GetGradeById(thisForm.Gradeid))?.Gradename ?? "";
            //var SubjectName = (await _catalogService.GetSubjectById(thisForm.Subjectid))?.Subjectname ?? "";
            //var address = await _addressService.GetDistrictData(thisForm.Districtid);
            //string AddressName = $"{address.Province.Provincename}, {address.Districtname}, {thisForm.Addressdetail}";


            TutorRequestProfileViewModel view = null!;
            //    new TutorRequestProfileViewModel()
            //{
            //    FormId = id,
            //    AddressName = AddressName,
            //    CreatedDate = DateOnly.FromDateTime(thisForm.Createddate),
            //    CurrentStatus = thisForm.Status.ToString(),
            //    ExpiredDate = DateOnly.FromDateTime(thisForm.Expireddate), 
            //    GradeName = GradeName,
            //    FullNameRequester = thisForm.Account.Fullname,
            //    SubjectName = SubjectName,
            //};

            //foreach(var tutor in tutorInQueue)
            //{
            //    Account account = await _authService.GetAccountById(tutor.Accountid);

                //view.TutorCards.Add(new Models.TutorViewModel.TutorCardViewModel()
                //{
                //    Id = tutor.Id,
                //    Avatar = account.Avatar,
                //    FullName = account.Fullname,
                //    Area = tutor.Area,
                //    College = tutor.College,
                //    TutorType = tutor.Currentstatus.ToString()
                //});
            //}
        
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ApplyTutorRequest(int id)
        {
            //var thisForm = await _tutorRequestService.GetTutorRequestFormById(id);
            //if (thisForm == null)
            //{
            //    TempData[AppConfig.MESSAGE_FAIL] = "Không tìm thấy thông tin đơn vui lòng làm lại";
            //    return RedirectToAction("TutorRequestQueue", "Employee");
            //}
            ////thisForm.Status = AppConfig.TutorRequestStatus.APPROVAL;
            //bool isSuccess = await _tutorRequestService.UpdateForm(thisForm);
            //if (isSuccess)
            //{
            //    TempData[AppConfig.MESSAGE_SUCCESS] = "Đơn được chấp thuận thành công";
            //}
            //else
            //{
            //    TempData[AppConfig.MESSAGE_FAIL] = "Lỗi hệ thống vui lòng làm lại";
            //}
            return RedirectToAction("TutorRequestQueue", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> DenyTutorRequest(int id)
        {
            //var thisForm = await _tutorRequestService.GetTutorRequestFormById(id);
            //if (thisForm == null)
            //{
            //    TempData[AppConfig.MESSAGE_FAIL] = "Không tìm thấy thông tin đơn vui lòng làm lại";
            //    return RedirectToAction("TutorRequestQueue", "Employee");
            //}
            ////thisForm.Status = AppConfig.TutorRequestStatus.DENY;
            //bool isSuccess = await _tutorRequestService.UpdateForm(thisForm);
            //if (isSuccess)
            //{
            //    TempData[AppConfig.MESSAGE_SUCCESS] = "Đã từ chối đơn thành công";
            //}
            //else
            //{
            //    TempData[AppConfig.MESSAGE_FAIL] = "Lỗi hệ thống vui lòng làm lại";
            //}
            return RedirectToAction("TutorRequestQueue", "Employee");
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
            ResponseService result = await _profileService.UpdateTutorProfile(tutorProfileViewModel);
            
            if (result.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                return RedirectToAction("TutorList", "Employee");
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = result.Message;
                return RedirectToAction("TutorProfile", "Employee", new {id = tutorProfileViewModel.TutorId});
            }
        }
    }
}
