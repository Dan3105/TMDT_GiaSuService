using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiaSuService.Controllers
{
    [Authorize(Policy = AppConfig.EMPLOYEEPOLICY)]
    public class EmployeeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly ITutorRequestFormService _tutorRequestService;

        public EmployeeController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService,
            ITutorRequestFormService tutorRequestService)
        {
            _tutorService = tutorService;
            _catalogService = catalogService;
            _addressService = addressService;
            _tutorRequestService = tutorRequestService;
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
            var listTutor = await _tutorRequestService.GetTutorrequestforms(AppConfig.TutorRequestStatus.PENDING);
            List<TutorRequestListViewModel> results = new List<TutorRequestListViewModel>();
            foreach(var tutor in listTutor)
            {
                string GradeName = (await _catalogService.GetGradeById(tutor.Gradeid))?.Gradename ?? "";
                var SubjectName = (await _catalogService.GetSubjectById(tutor.Subjectid))?.Subjectname ?? "";
                var address = await _addressService.GetDistrictData(tutor.Districtid);
                string AddressName = $"{address.Province.Provincename}, {address.Districtname}, {tutor.Addressdetail}";
                results.Add(new TutorRequestListViewModel
                { 
                    FormId = tutor.Id,
                    FullNameRequester = tutor.Account.Fullname,
                    AddressName = AddressName,
                    CreatedDate = DateOnly.FromDateTime(tutor.Createddate),
                    GradeName = GradeName,
                    SubjectName = SubjectName,
                });
            }
            return View(results);
        }
    }
}
