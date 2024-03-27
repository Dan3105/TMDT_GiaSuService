using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GiaSuService.Controllers
{
    [Authorize(Policy = AppConfig.EMPLOYEEPOLICY)]
    public class EmployeeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly ITutorRequestFormService _tutorRequestService;
        private readonly IAuthService _authService;


        public EmployeeController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService,
            ITutorRequestFormService tutorRequestService, IAuthService authService)
        {
            _tutorService = tutorService;
            _catalogService = catalogService;
            _addressService = addressService;
            _tutorRequestService = tutorRequestService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> TutorRegisterQueue()
        {
            List<Tutorprofile> queries = await _tutorService.GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus.PENDING);
            List<TutorRegisterViewModel> registers = new List<TutorRegisterViewModel>();
            foreach(var query in queries)
            {
                registers.Add(new TutorRegisterViewModel()
                {
                    Area = query.Area,
                    College = query.College,
                    CurrentStatus = query.Currentstatus.ToString(),
                    FullName = query.Account.Fullname,
                    Id = query.Id,
                    StatusQuery = query.Formstatus.ToString(),
                    CreateDate = query.Account.Createdate
                });
            }

            return View(registers);
        }

        [HttpGet]
        public async Task<IActionResult> Tutorprofile(int id)
        {
            Tutorprofile tutor = await _tutorService.GetTutorprofileById(id);
            if (tutor == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "User cannot be found";
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }
            District district = await _addressService.GetDistrictData(tutor.Account.Districtid);
            TutorProfileInEmployeeViewModel view = new TutorProfileInEmployeeViewModel()
            {
                Academicyearfrom = tutor.Academicyearfrom,
                Academicyearto = tutor.Academicyearto,
                Additionalinfo = tutor.Additionalinfo,
                Address = $"{district.Province.Provincename}, {district.Districtname}, {tutor.Account.Addressdetail}",
                Area = tutor.Area,
                Avatar = tutor.Account.Avatar,
                Backidentitycard = tutor.Account.Backidentitycard,
                Birth = tutor.Account.Birth,
                College = tutor.College,
                Createdate = tutor.Account.Createdate,  
                Currentstatus = tutor.Currentstatus.ToString(),
                Email = tutor.Account.Email,
                Formstatus = tutor.Formstatus,
                Frontidentitycard = tutor.Account.Frontidentitycard,
                Fullname = tutor.Account.Fullname,
                Gender = tutor.Account.Gender,
                Identitycard = tutor.Account.Identitycard,
                Lockenable = tutor.Account.Lockenable,
                Phone = tutor.Account.Phone,
                TutorId = tutor.Id,
            };
            return View(view);
        }

        [HttpGet]
        public async Task<IActionResult> ApplyTutor(int id)
        {
            Tutorprofile tutorprofile = await _tutorService.GetTutorprofileById(id);
            if (tutorprofile == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Sự cố phát sinh vui lòng làm lại";
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }

            tutorprofile.Account.Lockenable = false;
            tutorprofile.Formstatus = AppConfig.RegisterStatus.APPROVAL;

            bool isSuccess = await _tutorService.UpdateTutorprofile(tutorprofile);   
            if(!isSuccess) {
                TempData[AppConfig.MESSAGE_FAIL] = "Chấp thuận gia sư không thành công vui lòng làm lại";
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }
            TempData[AppConfig.MESSAGE_SUCCESS] = "Chấp thuận gia sư thành công!";
            return RedirectToAction("TutorRegisterQueue", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> DenyTutor(int id)
        {
            bool isSuccess = await _tutorService.UpdateTutorprofileStatus(id, AppConfig.RegisterStatus.DENY);
            if (!isSuccess)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Sự cố phát sinh vui lòng làm lại";
                return RedirectToAction("TutorRegisterQueue", "Employee");
            }
            TempData[AppConfig.MESSAGE_SUCCESS] = "Từ chối đơn tạo gia sư thành công!";
            return RedirectToAction("TutorRegisterQueue", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> TutorRequestQueue()
        {
            var listRequest = await _tutorRequestService.GetTutorrequestforms(AppConfig.TutorRequestStatus.PENDING);
            List<TutorRequestItemViewModel> results = new List<TutorRequestItemViewModel>();
            foreach(var request in listRequest)
            {
                string GradeName = (await _catalogService.GetGradeById(request.Gradeid))?.Gradename ?? "";
                var SubjectName = (await _catalogService.GetSubjectById(request.Subjectid))?.Subjectname ?? "";
                var address = await _addressService.GetDistrictData(request.Districtid);
                string AddressName = $"{address.Province.Provincename}, {address.Districtname}, {request.Addressdetail}";
                results.Add(new TutorRequestItemViewModel
                { 
                    FormId = request.Id,
                    FullNameRequester = request.Account.Fullname,
                    AddressName = AddressName,
                    CreatedDate = DateOnly.FromDateTime(request.Createddate),
                    GradeName = GradeName,
                    SubjectName = SubjectName,
                });
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
            var tutorInQueue = await _tutorService.GetTutorprofilesByClassId(id);

            string GradeName = (await _catalogService.GetGradeById(thisForm.Gradeid))?.Gradename ?? "";
            var SubjectName = (await _catalogService.GetSubjectById(thisForm.Subjectid))?.Subjectname ?? "";
            var address = await _addressService.GetDistrictData(thisForm.Districtid);
            string AddressName = $"{address.Province.Provincename}, {address.Districtname}, {thisForm.Addressdetail}";

            TutorRequestProfileViewModel view = new TutorRequestProfileViewModel()
            {
                FormId = id,
                AddressName = AddressName,
                CreatedDate = DateOnly.FromDateTime(thisForm.Createddate),
                CurrentStatus = thisForm.Status.ToString(),
                ExpiredDate = DateOnly.FromDateTime(thisForm.Expireddate), 
                GradeName = GradeName,
                FullNameRequester = thisForm.Account.Fullname,
                SubjectName = SubjectName,
            };

            foreach(var tutor in tutorInQueue)
            {
                Account account = await _authService.GetAccountById(tutor.Accountid);

                view.TutorCards.Add(new Models.TutorViewModel.TutorCardViewModel()
                {
                    Id = tutor.Id,
                    Avatar = account.Avatar,
                    FullName = account.Fullname,
                    Area = tutor.Area,
                    College = tutor.College,
                    TutorType = tutor.Currentstatus.ToString()
                });
            }
        
            return View(view);
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
            thisForm.Status = AppConfig.TutorRequestStatus.APPROVAL;
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
            thisForm.Status = AppConfig.TutorRequestStatus.DENY;
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
    }
}
