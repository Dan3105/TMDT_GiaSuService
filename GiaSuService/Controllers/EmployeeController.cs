using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
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
        private readonly IAuthService _authService;

        public EmployeeController(ITutorService tutorService)
        {
            _tutorService = tutorService;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeProfile(int id)
        {
            Account account = await _authService.GetAccountById(id);
            if (account == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Tdn mã nhân viên không tồn tại";
                return RedirectToAction("EmployeeList", "Admin");
            }

            //District district = await _addressService.GetDistrictData(account.Districtid);
            //DistrictViewModel district = await _addressService.GetDistrictData(1);
            //ProfileViewModel profile = null!;
            //    new EmployeeProfileViewModel()
            //{
            //    LogoAccount = account.Avatar,
            //    Phone = account.Phone,
            //    IdentityCard = account.Identitycard,
            //    FrontIdentiyCard = account.Frontidentitycard,
            //    BackIdentityCard = account.Backidentitycard,
            //    Gender = account.Gender,
            //    Email = account.Email,
            //    AddressDetail = district.Province.Provincename + " " + district.Districtname + " " + account.Addressdetail,
            //    FullName = account.Fullname,
            //    LockStatus = account.Lockenable,
            //    BirthDate = account.Birth,
            //    EmployeeId = account.Id
            //};
            return View();
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
            var thisForm = await _tutorRequestService.GetTutorRequestFormById(id);
            if (thisForm == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Không tìm thấy thông tin đơn vui lòng làm lại";
                return RedirectToAction("TutorRequestQueue", "Employee");
            }
            //thisForm.Status = AppConfig.TutorRequestStatus.APPROVAL;
            bool isSuccess = await _tutorRequestService.UpdateForm(thisForm);
            if (isSuccess)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Đơn được chấp thuận thành công";
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Lỗi hệ thống vui lòng làm lại";
            }
            return RedirectToAction("TutorRequestQueue", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> DenyTutorRequest(int id)
        {
            var thisForm = await _tutorRequestService.GetTutorRequestFormById(id);
            if (thisForm == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Không tìm thấy thông tin đơn vui lòng làm lại";
                return RedirectToAction("TutorRequestQueue", "Employee");
            }
            //thisForm.Status = AppConfig.TutorRequestStatus.DENY;
            bool isSuccess = await _tutorRequestService.UpdateForm(thisForm);
            if (isSuccess)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Đã từ chối đơn thành công";
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Lỗi hệ thống vui lòng làm lại";
            }
            return RedirectToAction("TutorRequestQueue", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> TutorList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorList(int subjectId, int districtId, int gradeId, int page)
        {
            var queries = await _tutorService.GetTutorAccountsByFilter(subjectId, districtId, gradeId, page);
            int totalPages = (int)Math.Ceiling((double)queries.Count / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> TutorProfile(int id)
        {
            Account account = await _authService.GetAccountById(id);
            if (account == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Tdn mã nhân viên không tồn tại";
                return RedirectToAction("TutorList", "Employee");
            }

            //District district = await _addressService.GetDistrictData(account.Districtid);
            //District district = await _addressService.GetDistrictData(1);
            //ProfileViewModel employeeProfileViewModel = null!;
            //new EmployeeProfileViewModel();
            //{
            //    LogoAccount = account.Avatar,
            //    Phone = account.Phone,
            //    IdentityCard = account.Identitycard,
            //    FrontIdentiyCard = account.Frontidentitycard,
            //    BackIdentityCard = account.Backidentitycard,
            //    Gender = account.Gender,
            //    Email = account.Email,
            //    AddressDetail = district.Province.Provincename + " " + district.Districtname + " " + account.Addressdetail,
            //    FullName = account.Fullname,
            //    LockStatus = account.Lockenable,
            //    BirthDate = account.Birth,
            //    EmployeeId = account.Id
            //};
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TutorProfile(ProfileViewModel employeeProfileViewModel)
        {
            Account? account = await _authService.GetAccountById(employeeProfileViewModel.EmployeeId);
            if (account == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Tdn mã nhân viên không tồn tại";
                return RedirectToAction("TutorList", "Employee");
            }

            ResponseService result = await _authService.UpdateAccount(account);
            if (result.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                return RedirectToAction("TutorList", "Employee");
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = result.Message;
                return RedirectToAction("TutorList", "Employee");
            }
        }
    }
}
