using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using System.Security.Cryptography;
namespace GiaSuService.Services
{
    public class AuthService : IAuthService
    {
        private readonly DvgsDbContext _context;
        private readonly IAccountRepository _accountRepo;
        private readonly ITutorRepository _tutorRepo;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IAddressRepository _addressRepository;

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

        public async Task<bool> CreateAccount(Account account)
        {
            try
            {
                _accountRepo.Create(account);
                var isSucced = await _accountRepo.SaveChanges();
                return isSucced;
            }
            catch(Exception)
            {
                return false;
            }
        }


        public async Task<bool> CreateTutorRegisterRequest(Account account, Tutor tutorprofile, IEnumerable<int> districtId, IEnumerable<int> gradeId, 
            IEnumerable<int> subjectId, IEnumerable<int> sessionId)
        {
            //using (var transaction = _context.Database.BeginTransactionhistory())
            //{
                try
                {
                    foreach(var id in districtId)
                    {
                        var district =await _addressRepository.GetDistrict(id);
                        tutorprofile.Districts.Add(district);
                    }

                    foreach (var id in gradeId)
                    {
                        var grade = await _gradeRepository.GetGradeById(id);
                        tutorprofile.Grades.Add(grade);
                    }

                    foreach (var id in subjectId)
                    {
                        var subject = await _subjectRepository.GetSubjectById(id);
                        tutorprofile.Subjects.Add(subject);
                    }

                    foreach (var id in sessionId)
                    {
                        var session = await _sessionRepository.GetSessionById(id);
                        tutorprofile.Sessions.Add(session);
                    }

                    _accountRepo.Create(account);
                    _tutorRepo.Create(tutorprofile);

                    await _context.SaveChangesAsync();

                    //transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    //transaction.Rollback();

                    return false;
                }
            //}
        }

        public async Task<Account> GetAccountById(int id)
        {
            Account account = await _accountRepo.GetById(id);
            return account;
        }

        public async Task<IEnumerable<Account>> GetAccountsByRole(string role)
        {
            int? roleId = await _accountRepo.GetRoleId(role);
            if(roleId == null)
            {
                return null!;
            }

            return await _accountRepo.GetAccountsByRoleId((int)roleId);
        }

        public async Task<int?> GetRoleId(string roleName)
        {
            return await _accountRepo.GetRoleId(roleName);
        }

        public async Task<bool> UpdateAccount(Account account)
        {
            try
            {
                _accountRepo.Update(account);
                var isSucced = await _accountRepo.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public async Task<Account> ValidateAccount(string email, string password)
        {
            Account account = await _accountRepo.GetByEmail(email);
            if (account == null)
            {
                return null!;
            }

            if(BCrypt.Net.BCrypt.Verify(password, account.Passwordhash))
            {
                return account;
            }

            return null!;
        }
    }
}
