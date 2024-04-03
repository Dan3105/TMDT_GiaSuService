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

        public async Task<int?> GetProfileId(int accountId, string roleName)
        {
            if (roleName.Equals(AppConfig.ADMINROLENAME.ToLower()) || roleName.Equals(AppConfig.EMPLOYEEROLENAME.ToLower()))
            {
                return (await _context.Employees.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Accountid == accountId))?.Id;

            }

            if (roleName.Equals(AppConfig.TUTORROLENAME.ToLower()))
            {
                return (await _context.Tutors.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Accountid == accountId))?.Id;
            }

            if (roleName.Equals(AppConfig.CUSTOMERROLENAME))
            {
                return (await _context.Customers.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Accountid == accountId))?.Id;
            }

            return null;
        }

        public async Task<Identitycard?> GetIdentitycard(string identityNumber)
        {
            return await _context.Identitycards.AsNoTracking().FirstOrDefaultAsync(x => x.Identitynumber == identityNumber);
        }

        public async Task<List<AccountListViewModel>> GetEmployeeList(int crrPage)
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

            return await query.ToListAsync();
        }


        #region Get_Profile_View_Model_Employee_Customer_Tutor
        public async Task<ProfileViewModel?> GetProfile(int profileId, string role)
        {
            if (role == null || role == "") return null;

            ProfileViewModel? profile = null;
            if(role == AppConfig.CUSTOMERROLENAME)
            {
                profile = await _context.Customers
                .AsNoTracking()
                .Select(p => new ProfileViewModel()
                {
                    ProfileId = p.Id,
                    IdentityId = p.Identityid,
                    AccountId = p.Accountid,
                    Avatar = p.Account.Avatar,
                    Email = p.Account.Email,
                    BirthDate = p.Birth,
                    FullName = p.Fullname,
                    LockStatus = p.Account.Lockenable ?? false,
                    Phone = p.Account.Phone,
                    Gender = (p.Gender == "M" ? "Nam" : "Nữ"),
                    IdentityCard = p.Identity.Identitynumber,
                    FrontIdentityCard = p.Identity.Frontidentitycard,
                    BackIdentityCard = p.Identity.Backidentitycard,

                    AddressDetail = p.Addressdetail,
                    SelectedDistrictId = p.Districtid,
                    SelectedProvinceId = p.District.Provinceid,
                })
                .FirstOrDefaultAsync(p => p.ProfileId == profileId);
            }
            else if (role == AppConfig.EMPLOYEEROLENAME || role == AppConfig.ADMINROLENAME)
            {
                profile = await _context.Employees
                .AsNoTracking()
                .Select(p => new ProfileViewModel()
                {
                    ProfileId = p.Id,
                    IdentityId = p.Identityid,
                    AccountId = p.Accountid,
                    Avatar = p.Account.Avatar,
                    Email = p.Account.Email,
                    BirthDate = p.Birth,
                    FullName = p.Fullname,
                    LockStatus = p.Account.Lockenable ?? false,
                    Phone = p.Account.Phone,
                    Gender = (p.Gender == "M" ? "Nam" : "Nữ"),
                    IdentityCard = p.Identity.Identitynumber,
                    FrontIdentityCard = p.Identity.Frontidentitycard,
                    BackIdentityCard = p.Identity.Backidentitycard,

                    AddressDetail = p.Addressdetail,
                    SelectedDistrictId = p.Districtid,
                    SelectedProvinceId = p.District.Provinceid,
                })
                .FirstOrDefaultAsync(p => p.ProfileId == profileId);
            }

            return profile;
        }

        public async Task<TutorProfileViewModel?> GetTutorProfile(int tutorId)
        {
            var profile = await _context.Tutors
                .Select(tutor => new TutorProfileViewModel
                {
                    TutorId = tutor.Id,
                    AccountId = tutor.Accountid,
                    IdentityId = tutor.Identityid,
                    Email = tutor.Account.Email,
                    Phone = tutor.Account.Phone,
                    Lockenable = tutor.Account.Lockenable ?? false,
                    Createdate = DateOnly.FromDateTime((DateTime)tutor.Account.Createdate!),
                    LockStatus = tutor.Account.Lockenable ?? false,
                    Avatar = tutor.Account.Avatar,

                    Fullname = tutor.Fullname,
                    Gender = tutor.Gender == "M" ? "Nam" : "Nữ",
                    AddressDetail = tutor.Addressdetail,
                    SelectedDistrictId = tutor.Districtid,
                    SelectedProvinceId = tutor.District.Provinceid,

                    Area = tutor.Area,
                    College = tutor.College,
                    Academicyearfrom = tutor.Academicyearfrom,
                    Academicyearto = tutor.Academicyearto,
                    Additionalinfo = tutor.Additionalinfo,
                    Birth = tutor.Birth,

                    TypeTutor = tutor.Typetutor,

                    IdentityCard = tutor.Identity.Identitynumber,
                    FrontIdentityCard = tutor.Identity.Frontidentitycard,
                    BackIdentityCard = tutor.Identity.Backidentitycard,
                })
                .FirstOrDefaultAsync(p => p.TutorId == tutorId);

            return profile;
        }
        #endregion

        #region Update_Profile_Employee_Customer_Tutor
        public async Task<bool> UpdateProfile(ProfileViewModel profile, string role)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Identitycard? identity = await _context.Identitycards.FindAsync(profile.IdentityId);
                    if (identity == null)
                    {
                        return false;
                    }
                    identity.Identitynumber = profile.IdentityCard;
                    identity.Backidentitycard = profile.BackIdentityCard;
                    identity.Frontidentitycard = profile.FrontIdentityCard;

                    Account? account = await _context.Accounts.FindAsync(profile.AccountId);
                    if (account == null)
                    {
                        return false;
                    }
                    account.Email = profile.Email;
                    account.Phone = profile.Phone;
                    account.Avatar = profile.Avatar;
                    account.Lockenable = profile.LockStatus;

                    _context.Identitycards.Update(identity);
                    _context.Accounts.Update(account);

                    if (role == AppConfig.EMPLOYEEROLENAME || role == AppConfig.ADMINROLENAME)
                    {
                        Employee? employee = await _context.Employees.FindAsync(profile.ProfileId);
                        if (employee == null)
                        {
                            return false;
                        }
                        employee.Fullname = profile.FullName;
                        employee.Addressdetail = profile.AddressDetail;
                        employee.Gender = profile.Gender == "Nam" ? "M" : "F";
                        employee.Birth = profile.BirthDate;
                        employee.Districtid = profile.SelectedDistrictId;
                        _context.Employees.Update(employee);
                    }
                    else
                    {
                        Customer? customer = await _context.Customers.FindAsync(profile.ProfileId);
                        if (customer == null)
                        {
                            return false;
                        }
                        customer.Fullname = profile.FullName;
                        customer.Addressdetail = profile.AddressDetail;
                        customer.Gender = profile.Gender == "Nam" ? "M" : "F";
                        customer.Birth = profile.BirthDate;
                        customer.Districtid = profile.SelectedDistrictId;
                        _context.Customers.Update(customer);
                    }

                    int result = _context.SaveChanges();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return false;
        }

        public async Task<bool> UpdateTutorProfile(TutorProfileViewModel profile)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Identitycard? identity = await _context.Identitycards.FindAsync(profile.IdentityId);
                    if (identity == null)
                    {
                        return false;
                    }
                    identity.Identitynumber = profile.IdentityCard;
                    identity.Backidentitycard = profile.BackIdentityCard;
                    identity.Frontidentitycard = profile.FrontIdentityCard;

                    Account? account = await _context.Accounts.FindAsync(profile.AccountId);
                    if (account == null)
                    {
                        return false;
                    }
                    account.Email = profile.Email;
                    account.Phone = profile.Phone;
                    account.Avatar = profile.Avatar;
                    account.Lockenable = profile.LockStatus;

                    Tutor? tutor = await _context.Tutors.FindAsync(profile.TutorId);
                    if (tutor == null)
                    {
                        return false;
                    }
                    tutor.Fullname = profile.Fullname;
                    tutor.Addressdetail = profile.AddressDetail;
                    tutor.Gender = profile.Gender == "Nam" ? "M" : "F";
                    tutor.Birth = profile.Birth;
                    tutor.Districtid = profile.SelectedDistrictId;
                    tutor.College = profile.College;
                    tutor.Area = profile.Area;
                    tutor.Additionalinfo = profile.Additionalinfo;
                    tutor.Academicyearfrom = profile.Academicyearfrom;
                    tutor.Academicyearto = profile.Academicyearto;
                    tutor.Typetutor = profile.TypeTutor;

                    _context.Identitycards.Update(identity);
                    _context.Accounts.Update(account);
                    _context.Tutors.Update(tutor);
                    int result = _context.SaveChanges();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return false;
        }

        #endregion
    
    }
}
