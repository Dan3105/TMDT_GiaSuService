using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace GiaSuService.Services
{
    public class AuthService : IAuthService
    {

        private DvgsDbContext _context;
        private readonly IAccountRepo _accountRepo;
        private readonly IProfileRepo _profileRepo;
        private readonly IStatusRepo _statusRepo;
        private readonly IUploadFileService _uploadFileService;

        public AuthService(DvgsDbContext context, IAccountRepo accountRepo, IProfileRepo profileRepo, IStatusRepo statusRepo, IUploadFileService uploadFileService)
        {
            _context = context;
            _accountRepo = accountRepo;
            _profileRepo = profileRepo;
            _statusRepo = statusRepo;
            _uploadFileService = uploadFileService;
        }

        #region Function to check before creating account
        public async Task<ResponseService> CheckEmailExist(string email)
        {
            Account? acc = await _accountRepo.GetByEmailOrPhone(email);
            if (acc == null)
            {
                return new ResponseService { Success = false, Message = "Email chưa tồn tại" };
            }
            return new ResponseService { Success = true, Message = "Email đã tồn tại" }; ;
        }

        public async Task<ResponseService> CheckPhoneExist(string phone)
        {
            Account? acc = await _accountRepo.GetByEmailOrPhone(phone);
            if (acc == null)
            {
                return new ResponseService { Success = false, Message = "Số điện thoại chưa tồn tại" };
            }
            return new ResponseService { Success = true, Message = "Số điện thoại đã tồn tại" };
        }

        public async Task<ResponseService> CheckIdentityCardExist(string identityNumber)
        {
            var identityCard = await _profileRepo.GetIdentitycard(identityNumber);
            if (identityCard == null)
            {
                return new ResponseService { Success = false, Message = "Chứng minh thư chưa tồn tại trong hệ thống" };
            }

            return new ResponseService { Success = true, Message = "Chứng minh thư đã tồn tại trong hệ thống" };
        }

        public async Task<ResponseService> CheckBeforeCreateAccount(string email, string phone, string identityNumber)
        {
            ResponseService rs = await CheckEmailExist(email);
            if (rs.Success)
            {
                return new ResponseService { Success = false, Message = "Email đã được sử dụng. Vui lòng thử email khác" };
            }

            rs = await CheckEmailExist(phone);
            if (rs.Success)
            {
                return new ResponseService { Success = false, Message = "Số điện thoại đã được sử dụng. Vui lòng thử số điện thoại khác" };
            }

            rs = await CheckIdentityCardExist(identityNumber);
            if (rs.Success)
            {
                return new ResponseService { Success = false, Message = "CMND/CCCD này đã được sử dụng. Vui lòng thử CMND/CCCD khác" };
            }

            return new ResponseService { Success = true, Message = "" };
        }

        #endregion

        public async Task<ResponseService> CreateAccount(Account account)
        {
            try
            {
                _accountRepo.Create(account);
                var isSuccess = await _accountRepo.SaveChanges();
                if (isSuccess)
                {
                    return new ResponseService { Success = isSuccess, Message = "Tạo tài khoản thành công" };
                }
            }
            catch (Exception)
            { }
            return new ResponseService { Success = false, Message = "Có lỗi trong hệ thống vui lòng làm lại" };
        }

        public async Task<ResponseService> CreateAccount(RegisterAccountProfileViewModel? accountProfile, IFormFile avatar, IFormFile frontCard, IFormFile backCard, string accountRole)
        {
            try
            {
                if (accountProfile == null)
                {
                    return new ResponseService { Success = false, Message = "Lỗi form đăng ký rỗng" };
                }

                #region Check_email_or_phone_or_identity_card_exist
                ResponseService rs = await CheckBeforeCreateAccount(accountProfile.Email, accountProfile.Phone, accountProfile.IdentityCard);
                if (!rs.Success)
                {
                    return new ResponseService { Success = false, Message = rs.Message };
                }
                #endregion

                var roleId = await GetRoleId(accountRole);
                if (roleId == null)
                {
                    return new ResponseService { Success = false, Message = "Lỗi hệ thống, vui lòng thử lại sau giây lát" };
                }

                #region Upload_image
                if (avatar != null)
                {
                    ResponseService response = await _uploadFileService.UploadFile(avatar, AppConfig.UploadFileType.AVATAR);
                    if (!response.Success) return response;
                    accountProfile.Avatar = response.Message;
                }

                if (frontCard != null)
                {
                    ResponseService response = await _uploadFileService.UploadFile(frontCard, AppConfig.UploadFileType.FRONT_IDENTITY_CARD);
                    if (!response.Success) return response;
                    accountProfile.FrontIdentityCard = response.Message;
                }

                if (backCard != null)
                {
                    ResponseService response = await _uploadFileService.UploadFile(backCard, AppConfig.UploadFileType.BACK_IDENTITY_CARD);
                    if (!response.Success) return response;
                    accountProfile.BackIdentityCard = response.Message;
                }
                #endregion

                #region Create_account
                Account account = new Account();
                if(accountRole == AppConfig.CUSTOMERROLENAME)
                {
                    account = new Account()
                    {
                        Email = accountProfile.Email,
                        Phone = accountProfile.Phone,
                        PasswordHash = Utility.HashPassword(accountProfile.Password),
                        Avatar = accountProfile.Avatar,
                        CreateDate = DateTime.Now,
                        LockEnable = false,
                        RoleId = (int)roleId,

                        Customer = new Customer()
                        {
                            FullName = Utility.FormatToCamelCase(accountProfile.FullName),
                            Birth = accountProfile.BirthDate,
                            Gender = accountProfile.Gender,
                            AddressDetail = Utility.FormatToCamelCase(accountProfile.AddressName),
                            DistrictId = accountProfile.SelectedDistrictId,

                            Identity = new IdentityCard()
                            {
                                IdentityNumber = accountProfile.IdentityCard,
                                FrontIdentityCard = accountProfile.FrontIdentityCard,
                                BackIdentityCard = accountProfile.BackIdentityCard,
                            }
                        }
                    };
                }
                else
                {   // If creating account for new employee
                    account = new Account()
                    {
                        Email = accountProfile.Email,
                        Phone = accountProfile.Phone,
                        LockEnable = false,
                        Avatar = accountProfile.Avatar,
                        RoleId = (int)roleId,
                        CreateDate = DateTime.Now,
                        PasswordHash = Utility.HashPassword(accountProfile.Password),

                        Employee = new Employee()
                        {
                            FullName = Utility.FormatToCamelCase(accountProfile.FullName),
                            Birth = accountProfile.BirthDate,
                            Gender = accountProfile.Gender,
                            AddressDetail = Utility.FormatToCamelCase(accountProfile.AddressName),
                            DistrictId = accountProfile.SelectedDistrictId,
                            Identity = new IdentityCard()
                            {
                                IdentityNumber = accountProfile.IdentityCard,
                                FrontIdentityCard = accountProfile.FrontIdentityCard,
                                BackIdentityCard = accountProfile.BackIdentityCard,
                            }
                        }
                    };
                }
                #endregion

                _accountRepo.Create(account);
                var isSuccess = await _accountRepo.SaveChanges();
                if (isSuccess)
                {
                    return new ResponseService { Success = isSuccess, Message = "Tạo tài khoản thành công" };
                }

            }
            catch (Exception)
            { }
            return new ResponseService { Success = false, Message = "Có lỗi trong hệ thống vui lòng làm lại" };
        }
        public async Task<ResponseService> CreateTutorAccount(FormRegisterTutorRequestViewModel? model, IFormFile avatar, IFormFile frontCard, IFormFile backCard)
        {
            try
            {
                if (model == null)
                {
                    return new ResponseService { Success = false, Message = "Lỗi form đăng ký rỗng" };
                }

                #region Check_email_or_phone_exist
                ResponseService rs = await CheckBeforeCreateAccount(model.AccountProfile.Email, model.AccountProfile.Phone, model.AccountProfile.IdentityCard);
                if (!rs.Success)
                {
                    return new ResponseService { Success = false, Message = rs.Message };
                }
                #endregion

                var roleId = await GetRoleId(AppConfig.TUTORROLENAME);
                if (roleId == null)
                {
                    return new ResponseService { Success = false, Message = "Lỗi hệ thống, vui lòng thử lại sau giây lát" };
                }

                #region Upload_image
                if (avatar != null)
                {
                    ResponseService response = await _uploadFileService.UploadFile(avatar, AppConfig.UploadFileType.AVATAR);
                    if (!response.Success) return response;
                    model.AccountProfile.Avatar = response.Message;
                }

                if (frontCard != null)
                {
                    ResponseService response = await _uploadFileService.UploadFile(frontCard, AppConfig.UploadFileType.FRONT_IDENTITY_CARD);
                    if (!response.Success) return response;
                    model.AccountProfile.FrontIdentityCard = response.Message;
                }

                if (backCard != null)
                {
                    ResponseService response = await _uploadFileService.UploadFile(backCard, AppConfig.UploadFileType.BACK_IDENTITY_CARD);
                    if (!response.Success) return response;
                    model.AccountProfile.BackIdentityCard = response.Message;
                }
                #endregion

                var listGrade = model.GetGradeSelected.Select(p => p.GradeId);
                var listSession = model.GetSessionSelected.Select(p => p.SessionId);
                var listSubject = model.GetSubjectSelected.Select(p => p.SubjectId);

                #region Create_account
                Account account = new Account()
                {
                    Email = model.AccountProfile.Email,
                    Phone = model.AccountProfile.Phone,
                    PasswordHash = Utility.HashPassword(model.AccountProfile.Password),
                    RoleId = (int)roleId,
                    LockEnable = false,
                    Avatar = model.AccountProfile.Avatar,
                    CreateDate = DateTime.Now,
                    Tutor = new Tutor()
                    {
                        Birth = model.AccountProfile.BirthDate,
                        FullName = Utility.FormatToCamelCase(model.AccountProfile.FullName),
                        AddressDetail = Utility.FormatToCamelCase(model.AccountProfile.AddressName),
                        DistrictId = model.AccountProfile.SelectedDistrictId,
                        Gender = model.AccountProfile.Gender,

                        //Hoc van
                        AcademicYearFrom = model.RegisterTutorProfile.AcademicYearFrom,
                        AcademicYearTo = model.RegisterTutorProfile.AcademicYearto,
                        AdditionalInfo = model.RegisterTutorProfile.AdditionalInfo,
                        College = Utility.FormatToCamelCase(model.RegisterTutorProfile.College),
                        Area = Utility.FormatToCamelCase(model.RegisterTutorProfile.Area),
                        TutorTypeId = model.RegisterTutorProfile.TypeTutorId,

                        IsActive = true,
                        //Isvalid = false,

                        Identity = new IdentityCard()
                        {
                            IdentityNumber = model.AccountProfile.IdentityCard,
                            FrontIdentityCard = model.AccountProfile.FrontIdentityCard,
                            BackIdentityCard = model.AccountProfile.BackIdentityCard,
                        },
                    }

                };
                #endregion

                var result = await CreateTutorRegisterRequest(account, listSession, listSubject, listGrade,
                    model.ListDistrict);
                return result;
            }
            catch (Exception)
            { }
            return new ResponseService { Success = false, Message = "Có lỗi trong hệ thống vui lòng làm lại" };
        }

        public async Task<ResponseService> CreateTutorRegisterRequest(Account account, IEnumerable<int> sessionIds, IEnumerable<int> subjectIds, IEnumerable<int> gradeIds,
            IEnumerable<int> districtIds)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Check identity card
                    /*var existsingIdentity = await _profileRepo.GetIdentitycard(account.Tutor!.Identity.IdentityNumber);
                    if (existsingIdentity != null)
                    {
                        return new ResponseService { Success = false, Message = "Chứng minh thư đã tồn tại trong hệ thống" };
                    }*/
                    var status = await _statusRepo.GetStatus(AppConfig.RegisterStatus.PENDING.ToString().ToLower(), AppConfig.register_status);
                    if (status == null)
                    {
                        return new ResponseService { Success = false, Message = "Hệ thống không tạo được trạng thái vui lòng làm lại" };
                    }

                    account.Tutor.StatusId = status.Id;

                    account.Tutor.TutorStatusDetails.Add(new TutorStatusDetail()
                    {
                        Context = "Tạo tài khoản",
                        StatusId = status.Id,
                        CreateDate = DateTime.Now,
                    });

                    foreach (var id in sessionIds)
                    {
                        var session = await _context.SessionDates.FindAsync(id);
                        if (session != null)
                        {
                            account.Tutor.Sessions.Add(session);
                        }
                    }

                    foreach (var id in gradeIds)
                    {
                        var grade = await _context.Grades.FindAsync(id);
                        if (grade != null)
                        {
                            account.Tutor.Grades.Add(grade);
                        }
                    }

                    foreach(var id in subjectIds)
                    {
                        var subject = await _context.Subjects.FindAsync(id);
                        if(subject != null)
                        {
                            account.Tutor.Subjects.Add(subject);
                        }
                    }

                    foreach (var id in districtIds)
                    {
                        var district = await _context.Districts.FindAsync(id);
                        if (district != null)
                        {
                            account.Tutor.Districts.Add(district);
                        }
                    }
                    _accountRepo.Create(account);
                    bool isSuccess = await _accountRepo.SaveChanges();
                    if (isSuccess)
                    {
                        transaction.Commit();
                        return new ResponseService { Success = isSuccess, Message = "Tạo tài khoản gia sư thành công" };
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return new ResponseService { Success = false, Message = "Có lỗi trong hệ thống vui lòng làm lại" };
        }

        public async Task<Account?> GetAccountById(int id)
        {
            Account? account = await _accountRepo.GetById(id);
            return account;
        }

        public async Task<IEnumerable<Account>> GetAccountsByRole(string role)
        {
            int? roleId = await _accountRepo.GetRoleId(role);
            if (roleId == null)
            {
                return null!;
            }

            return await _accountRepo.GetAccountsByRoleId((int)roleId);
        }

        public async Task<int?> GetRoleId(string roleName)
        {
            return await _accountRepo.GetRoleId(roleName);
        }

        public async Task<ResponseService> UpdateAccount(Account account)
        {
            try
            {
                _accountRepo.Update(account);
                var isSuccess = await _accountRepo.SaveChanges();
                if (isSuccess)
                {
                    return new ResponseService { Success = isSuccess, Message = "Cập nhật tài khoản thành công" };
                }
            }
            catch (Exception)
            { }
            return new ResponseService { Success = false, Message = "Có lỗi trong hệ thống vui lòng làm lại" };

        }

        public async Task<Account?> ValidateAccount(string loginmail, string password)
        {
            Account? account = await _accountRepo.GetByEmailOrPhone(loginmail);
            if (account == null)
            {
                return null;
            }

            if (BCrypt.Net.BCrypt.Verify(password, account.PasswordHash))
            {
                return account;
            }

            return null;
        }

        public async Task<ResponseService> UpdatePassword(int accountId, string password)
        {
            try
            {
                bool isSuccess = await _accountRepo.UpdatePassword(accountId, password);
                if (isSuccess)
                {
                    return new ResponseService { Success = true, Message = "Đổi mật khẩu thành công" };
                }
                else
                {
                    return new ResponseService { Success = true, Message = "Đổi mật khẩu thất bại" };
                }
            }
            catch (Exception)
            { }
            return new ResponseService { Success = false, Message = "Lỗi hệ thống, vui lòng thử lại sau ít phút nữa" };

        }
    }
}
