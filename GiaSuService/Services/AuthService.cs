using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Security.Cryptography;
namespace GiaSuService.Services
{
    public class AuthService : IAuthService
    {

        private DvgsDbContext _context;
        private readonly IAccountRepo _accountRepo;
        private readonly IProfileRepo _profileRepo;
        private readonly IStatusRepo _statusRepo;

        //public AuthService(DvgsDbContext context, IAccountRepository accountRepo, ITutorRepository tutorRepo, ISubjectRepository subjectRepository, IGradeRepository gradeRepository, ISessionRepository sessionRepository, IAddressRepository addressRepository)
        //{
        //    _context = context;
        //    _accountRepo = accountRepo;
        //    _tutorRepo = tutorRepo;
        //    _subjectRepository = subjectRepository;
        //    _gradeRepository = gradeRepository;
        //    _sessionRepository = sessionRepository;
        //    _addressRepository = addressRepository;
        //}

        public AuthService(DvgsDbContext context, IAccountRepo accountRepo, IProfileRepo profileRepo, IStatusRepo statusRepo)
        {
            _context = context;
            _accountRepo = accountRepo;
            _profileRepo = profileRepo;
            _statusRepo = statusRepo;
        }

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


        public async Task<ResponseService> CreateTutorRegisterRequest(Account account, IEnumerable<int> sessionIds, IEnumerable<int> subjectIds, IEnumerable<int> gradeIds,
            IEnumerable<int> districtIds)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Check
                    var existsingIdentity = await _profileRepo.GetIdentitycard(account.Tutor!.Identity.IdentityNumber);
                    if (existsingIdentity != null)
                    {
                        return new ResponseService { Success = false, Message = "Chứng minh thư đã tồn tại trong hệ thống" };
                    }
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
    }
}
