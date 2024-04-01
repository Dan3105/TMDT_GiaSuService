using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class ProfileRepo : IProfileRepo
    {
        private readonly DvgsDbContext _context;
        public ProfileRepo(DvgsDbContext context)
        {
            _context = context;
        }


        public async Task<Identitycard?> GetIdentitycard(string identityNumber)
        {
            return await _context.Identitycards.AsNoTracking().FirstOrDefaultAsync(x => x.Identitynumber == identityNumber);
        }

        public Task<List<AccountListViewModel>> GetEmployeeList(int crrPage)
        {
            IQueryable<AccountListViewModel> query = _context.Employees
                .AsNoTracking()
                .Where(p => p.Account.Roleid != 1)
                .Select(p => new AccountListViewModel()
                {
                    Email = p.Account.Email,
                    FullName = p.Fullname,
                    Id = p.Id,
                    ImageUrl = p.Account.Avatar,
                    LockStatus = p.Account.Lockenable ?? false,
                })
                .OrderBy(p => p.Id)
                ;

            query = query.Skip(crrPage * AppConfig.ROWS_ACCOUNT_LIST)
                    .Take(AppConfig.ROWS_ACCOUNT_LIST);

            return query.ToListAsync();
        }

        public async Task<ProfileViewModel?> GetEmployeeProfile(int empId)
        {
            ProfileViewModel? query = await _context.Employees
                .AsNoTracking()
                .Select(p => new ProfileViewModel()
                {
                    IdentityId = p.Identityid,
                    EmployeeId = p.Id,
                    AccountId = p.Accountid,
                    LogoAccount = p.Account.Avatar,
                    Email = p.Account.Email,
                    BirthDate = p.Birth,
                    FullName = p.Fullname,
                    LockStatus = p.Account.Lockenable ?? false,
                    Phone = p.Account.Phone,
                    Gender = p.Gender == "M" ? "Nam" : "Nữ",
                    IdentityCard = p.Identity.Identitynumber,
                    FrontIdentiyCard = p.Identity.Frontidentitycard,
                    BackIdentityCard = p.Identity.Backidentitycard,

                    AddressDetail = $"{p.District.Province.Name}, {p.District.Name}, {p.Addressdetail}",
                })
                .FirstOrDefaultAsync(p => p.EmployeeId == empId);

            return query;

        }

        public async Task<bool> UpdateEmployeProfile(ProfileViewModel employeeProfile)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    Identitycard? identity = await _context.Identitycards.FindAsync(employeeProfile.IdentityId);
                    if (identity == null)
                    {
                        return false;
                    }
                    identity.Identitynumber = employeeProfile.IdentityCard;
                    identity.Backidentitycard = employeeProfile.BackIdentityCard;
                    identity.Frontidentitycard = employeeProfile.FrontIdentiyCard;

                    Account? account = await _context.Accounts.FindAsync(employeeProfile.AccountId);
                    if(account == null)
                    {
                        return false;
                    }
                    account.Lockenable = employeeProfile.LockStatus;

                    _context.Identitycards.Update(identity);
                    _context.Accounts.Update(account);
                    int result = _context.SaveChanges();
                    transaction.Commit();
                    return result > 0;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return false;
        }

        public async Task<TutorProfileViewModel?> GetTutorProfile(int tutorId)
        {

            var result = await _context.Tutors
                .Select(tutor => new TutorProfileViewModel
                {
                    TutorId = tutor.Id,
                    Email = tutor.Account.Email,
                    Phone = tutor.Account.Phone,
                    Lockenable = tutor.Account.Lockenable ?? false,
                    Createdate = DateOnly.FromDateTime((DateTime)tutor.Account.Createdate!),  
                    Avatar = tutor.Account.Avatar,

                    Fullname = tutor.Fullname,
                    Gender = tutor.Gender == "M" ? "Nam" : "Nữ",
                    Address = $"{tutor.District.Province.Name}, {tutor.District.Name}, {tutor.Addressdetail}",
                    
                    Area = tutor.Area,
                    College = tutor.College,
                    Academicyearfrom = tutor.Academicyearfrom,
                    Academicyearto = tutor.Academicyearto,
                    Additionalinfo = tutor.Additionalinfo,
                    Birth = tutor.Birth,
                    
                    TypeTutor = tutor.Typetutor ? "Giáo viên" : "Phụ huynh",

                    Identitycard = tutor.Identity.Identitynumber,
                    Frontidentitycard = tutor.Identity.Frontidentitycard,
                    Backidentitycard = tutor.Identity.Backidentitycard,
                })
                .FirstOrDefaultAsync(p => p.TutorId == tutorId);
            return result;
        }
    }
}
